using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskOneDraft.Areas.Identity.Data;
using TaskOneDraft.Controllers;
using TaskOneDraft.Models;
using Xunit;

namespace TestProject1
{
    public class ClaimRejectionTest
    {
        private readonly ClaimsController _controller;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Mock<IWebHostEnvironment> _mockEnv;

        public ClaimRejectionTest()
        {
            //set up in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TaskOneDraftBraden")
                .Options;

            _context = new ApplicationDbContext(options); //initialize the database context

            //set up the real UserManager with in-memory storage
            var userStore = new UserStore<ApplicationUser>(_context);
            _userManager = new UserManager<ApplicationUser>(
                userStore,
                null,
                new PasswordHasher<ApplicationUser>(),
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,
                null
            );

            //mock the hosting environment
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(env => env.WebRootPath).Returns("C:\\fakepath"); //set up fake web root path

            //seed a test user in the database
            var testUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),  //use a guid for unique user ids
                Email = "daisy@cmcs.com", //set email
                UserName = "daisy@cmcs.com" //set username
            };
            _userManager.CreateAsync(testUser).Wait(); //create the test user asynchronously

            //initialize the controller with the real context, user manager, and mocks
            _controller = new ClaimsController(_context, _mockEnv.Object, _userManager);

            //set up fake user for HttpContext
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, testUser.Id),  //use the id of the test user
                new Claim(ClaimTypes.Name, "TestingUser") //set the user's name as "TestUser"
            };
            var claimsIdentity = new ClaimsIdentity(userClaims, "TestingAuth"); //set up claims identity
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity); //create a principal with the identity

            //set HttpContext.User for the controller
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal //set the fake user for HttpContext
                }
            };
        }

        [Fact]
        public async Task UpdateClaimStatus_RejectClaim_ChangesToPendingInDb()
        {
            //arrange: seed a claim with "processed" status
            var claim = new Claims
            {
                LecturerID = Guid.NewGuid().ToString(),  //use a unique lecturer id to avoid collision
                FirstName = "Caleb", //set first name
                LastName = "Braden", //set last name
                Email = "caleb28@gmail.com", //set email
                ClaimsPeriodStart = DateTime.Now.AddDays(-7), //set start date 7 days ago
                ClaimsPeriodEnd = DateTime.Now, //set end date as today
                HoursWorked = 5, //set hours worked
                RateHour = 5, //set rate per hour
                TotalAmount = 25, //set total amount
                DescriptionOfWork = "Testing", //set description of work
                SupportingDocuments = "dummyDocument.pdf",  //mock supporting document
                UserID = Guid.NewGuid().ToString(),  //use a unique user id
                ClaimStatus = "Processed",  //initial status is processed
                DateSubmitted = DateTime.Now //set current date for submission
            };

            _context.Claims.Add(claim); //add claim to the context
            await _context.SaveChangesAsync(); //save changes asynchronously

            //act: change the status back to "pending"
            await _controller.UpdateClaimStatus(claim.ID, "Pending");

            //assert: verify that the claim's status is now "pending" in the database
            var updatedClaim = await _context.Claims.FindAsync(claim.ID); //fetch the claim from the db
            Assert.NotNull(updatedClaim); //ensure claim is found
            Assert.Equal("Pending", updatedClaim.ClaimStatus); //check that status is updated to "pending"
        }
    }
}
