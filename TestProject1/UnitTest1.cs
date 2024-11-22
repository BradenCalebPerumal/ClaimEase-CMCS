using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskOneDraft.Areas.Identity.Data;
using TaskOneDraft.Controllers;
using TaskOneDraft.Models;
using Xunit;

namespace TestProject1
{
    public class ClaimSubmitTest
    {
        private readonly ClaimsController _controller;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Mock<IWebHostEnvironment> _mockEnv;

        public ClaimSubmitTest()
        {
            //set up in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TaskOneDraftBraden")
                .Options;

            _context = new ApplicationDbContext(options);

            //set up the real UserManager with in-memory storage
            var userStore = new UserStore<ApplicationUser>(_context);
            var userManagerOptions = new IdentityOptions();
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
            _mockEnv.Setup(env => env.WebRootPath).Returns("C:\\fakepath");

            //seed a test user in the database
            var testUser = new ApplicationUser
            {
                Id = "181", //unique id for the user
                Email = "john.cmcs.com",
                UserName = "john.cmcs.com"
            };
            _userManager.CreateAsync(testUser).Wait(); //create user asynchronously

            //initialize the controller with the real context, user manager, and mocks
            _controller = new ClaimsController(_context, _mockEnv.Object, _userManager);

            //set up fake user for HttpContext
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "181"), //user id claim
                new Claim(ClaimTypes.Name, "TestingUser") //name claim
            };
            var claimsIdentity = new ClaimsIdentity(userClaims, "TestingAuth"); //identity with claims
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity); //principal with identity

            //set HttpContext.User for the controller
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal //assign fake user to HttpContext
                }
            };
        }

        [Fact]
        public async Task ClaimsPost_ValidModelWithFile_RedirectToClaimSummary()
        {
            //arrange: prepare a valid claim model with all necessary details
            var claims = new Claims
            {
                LecturerID = "1001", //lecturer id
                FirstName = "Nikhile", //first name
                LastName = "Ishkar", //last name
                Email = "nikish@gmail.com", //email address
                ClaimsPeriodStart = DateTime.Now.AddDays(-7), //start date of the claim period
                ClaimsPeriodEnd = DateTime.Now, //end date of the claim period
                HoursWorked = 5, //hours worked
                RateHour = 500, //rate per hour
                TotalAmount = 25, //total amount calculated
                DescriptionOfWork = "Testing Db entry", //description of work
                SupportingDocuments = "test_file_123", //mock supporting document identifier
                UserID = "181", //id of the user submitting the claim
                ClaimStatus = "Pending", //initial claim status
                DateSubmitted = DateTime.Now //current date for submission
            };

            //mock the uploaded file
            var mockFile = new Mock<IFormFile>();

            //set up the stream with known length and content
            var content = "This is a test file."; //sample file content
            var fileName = "test.pdf"; //sample file name
            var memoryStream = new MemoryStream(); //in-memory stream for the file
            var writer = new StreamWriter(memoryStream); //stream writer for the memory stream
            writer.Write(content); //write content to the stream
            writer.Flush(); //flush the writer to ensure the content is written
            memoryStream.Position = 0; //reset stream position to the beginning

            //set up the mock file properties
            mockFile.Setup(f => f.FileName).Returns(fileName); //return the file name
            mockFile.Setup(f => f.Length).Returns(memoryStream.Length); //return the file length
            mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream); //return the file stream
            mockFile.Setup(f => f.ContentType).Returns("application/pdf"); //set file content type
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                    .Returns<Stream, CancellationToken>((stream, cancellationToken) => memoryStream.CopyToAsync(stream, cancellationToken)); //mock copying of the file to the stream

            var result = await _controller.Claims(claims, mockFile.Object);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result); //assert the action result type
            Assert.Equal("ClaimSummary", redirectResult.ActionName); //assert the action name is ClaimSummary

            Assert.Equal(3, _context.Claims.Count()); //verify that 3 claims exist in the database (assuming 2 are already there)
        }
    }
}
