using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskOneDraft.Areas.Identity.Data;
using TaskOneDraft.Models;

namespace TaskOneDraft.Controllers
{
    public class ClaimsController : Controller
    {

        //variables for db and hosting
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        //constructor initializing context, web host environment, and user manager
        public ClaimsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager; //initializing user manager
            _context = context; //initializing database context
            this.webHostEnvironment = webHostEnvironment; //initializing web host environment
        }

        //action to view lecturers
        public async Task<IActionResult> ViewLecturers()
        {
            var users = await _userManager.Users.ToListAsync(); //fetching all users
            var lecturers = new List<IdentityUser>(); //list to store lecturers

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); //fetch roles of each user
                if (!roles.Contains("Admin")) //exclude users with the "Admin" role
                {
                    lecturers.Add(user); //add non-admin users to lecturers list
                }
            }

            return View(lecturers); //pass the filtered list of users to the view
        }

        //get: display claims for status update
        public async Task<IActionResult> UpdateClaimStatus()
        {
            var claims = await _context.Claims.ToListAsync(); //fetch all claims
            return View(claims); //display claims in the view
        }

        //post: update the claim status
        [HttpPost]
        public async Task<IActionResult> UpdateClaimStatus(int claimId, string newStatus)
        {
            try
            {
                Console.WriteLine($"Attempting to update claim status. Claim ID: {claimId}, New Status: {newStatus}"); //log attempt to update status

                //check if the new status is not empty
                if (!string.IsNullOrEmpty(newStatus))
                {
                    //find the claim by id
                    var claim = await _context.Claims.FindAsync(claimId);
                    if (claim != null)
                    {
                        Console.WriteLine($"Claim found. Current Status: {claim.ClaimStatus}"); //log current status

                        //update the status
                        claim.ClaimStatus = newStatus;

                        //update claim in the context
                        _context.Update(claim);

                        //save changes
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"Claim status updated to: {claim.ClaimStatus}"); //log updated status

                        //return success response
                        return Json(new { success = true, message = "Claim status updated successfully." });
                    }
                    else
                    {
                        Console.WriteLine($"Claim not found with ID: {claimId}"); //log if claim not found
                    }
                }
                else
                {
                    Console.WriteLine("New status is null or empty."); //log if new status is empty
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}"); //log error message
                Console.WriteLine(ex.StackTrace); //log stack trace for error analysis
            }

            //return error response if claim not found or status is invalid
            return Json(new { success = false, message = "Failed to update claim status. Please try again." });
        }

        //get: display claims with userid
        public async Task<IActionResult> ViewAllClaims()
        {
            var claims = await _context.Claims.ToListAsync(); //fetch all claims
            return View(claims); //pass claims to the view
        }

        //post: update user information
        [HttpPost]
        public async Task<IActionResult> UpdateUser(string userId, string newEmail, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId); //find user by id
            if (user != null)
            {
                if (!string.IsNullOrEmpty(newEmail) && user.Email != newEmail) //check if new email is provided and different
                {
                    user.Email = newEmail;
                    user.UserName = newEmail; //update username if tied to email
                    var emailResult = await _userManager.UpdateAsync(user); //update user email
                    if (!emailResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Error updating email."); //add error if email update fails
                    }
                }

                if (!string.IsNullOrEmpty(newPassword)) //check if new password is provided
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user); //generate password reset token
                    var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword); //reset password
                    if (!passwordResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Error updating password."); //add error if password update fails
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found."); //add error if user not found
            }

            //redirect back to the list with updated data
            return RedirectToAction(nameof(ViewLecturers));
        }

        //http get
        [HttpGet]
        public IActionResult Claims()
        {
            return View(); //display claims view
        }

        //action to display faqs
        public IActionResult FAQS()
        {
            return View(); //display faqs view
        }
        public IActionResult GenerateReport()
        {
            return View(); //display faqs view
        }

        //[authorize(roles = "lecturer")]  ---if you want to restrict who can use it
        public async Task<IActionResult> List()
        {
            var claims = await _context.Claims.ToListAsync(); //fetch all claims
            return View(claims); //ensure this matches the name of your view
        }

        //action to view claim status for logged-in lecturer
        public async Task<IActionResult> ViewClaimStatus()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //get logged-in lecturer's userid

            var claims = _context.Claims.Where(c => c.UserID == userId).ToList(); //retrieve claims associated with logged-in lecturer

            return View(claims); //pass claims to the view
        }

        //post: submit claims
[HttpPost]
public async Task<IActionResult> Claims(Claims claims, IFormFile supportingDocument)
{
    Console.WriteLine("Claims action called"); // Log action call

    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assign logged-in lecturer's userId
    claims.UserID = userId;
    Console.WriteLine($"Assigned UserID: {claims.UserID}"); // Log assigned userId
    ModelState.Remove("UserID");
    ModelState.Remove("Email");
    claims.ClaimStatus = "Pending"; // Set claim status to "pending"
    ModelState.Remove("ClaimStatus");
    ModelState.Remove("DateSubmitted");

    // Handling the supporting document
    if (supportingDocument != null && supportingDocument.Length > 0)
    {
        using (var ms = new MemoryStream())
        {
            await supportingDocument.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            // Convert the file to a base64 string
            string base64String = Convert.ToBase64String(fileBytes).Substring(0, 15);
            string fileExtension = Path.GetExtension(supportingDocument.FileName);

            string distinctPrefix = $"{claims.FirstName}_{claims.LastName}_[{claims.DateSubmitted}]";
            claims.SupportingDocuments = distinctPrefix + base64String + fileExtension;
        }
    }
    else
    {
        claims.SupportingDocuments = null; // Handle case where no document is provided
        Console.WriteLine("No supporting documents provided or file is empty.");
    }

    ModelState.Remove("supportingDocument");
    ModelState.Remove("SupportingDocuments");

    if (string.IsNullOrEmpty(userId))
    {
        ModelState.AddModelError("UserID", "User must be logged in to submit a claim."); // Add error if userId not found
        return View(claims);
    }

    // Retrieve user's email from the UserManager
    var user = await _userManager.FindByIdAsync(userId);
    if (user != null)
    {
        claims.Email = user.Email; // Set email from the current logged-in user
    }
    else
    {
        ModelState.AddModelError("Email", "Failed to retrieve user email.");
        return View(claims);
    }

    if (ModelState.IsValid)
    {
        Console.WriteLine("ModelState is valid"); // Log valid model state
        claims.DateSubmitted = DateTime.Now;

        // Calculate total amount
        claims.TotalAmount = claims.HoursWorked * claims.RateHour;

        // Add overtime to total if applicable
        if (claims.OvertimeHours.HasValue && claims.OvertimeRate.HasValue)
        {
            claims.TotalAmount += claims.OvertimeHours.Value * claims.OvertimeRate.Value;
        }

        _context.Add(claims); // Add claim to database
        await _context.SaveChangesAsync(); // Save changes
        Console.WriteLine("Claim saved successfully"); // Log claim saved

        //*********************************************************************************************

        // Handle the supporting document
        if (supportingDocument != null && supportingDocument.Length > 0)
        {
            Console.WriteLine($"File '{supportingDocument.FileName}' detected. Size: {supportingDocument.Length} bytes.");

            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".docx", ".xlsx", ".pdf" };
            var extension = Path.GetExtension(supportingDocument.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
            {
                Console.WriteLine("Invalid file type detected.");
                ModelState.AddModelError("", "Invalid file type.");
                return View(claims);
            }

            var mimeType = supportingDocument.ContentType;
            var permittedMimeTypes = new[]
            {
                "image/jpeg",  // For .jpg and .jpeg
                "image/png",   // For .png
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // For .docx
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",       // For .xlsx
                "application/pdf"  // For .pdf
            };
            if (!permittedMimeTypes.Contains(mimeType))
            {
                Console.WriteLine("Invalid MIME type detected.");
                ModelState.AddModelError("", "Invalid MIME type.");
                return View(claims);
            }

            // Ensure the uploads directory exists
            var uploadsFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
                Console.WriteLine("Uploads directory created.");
            }

            // Generate a unique file name
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(supportingDocument.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            // Save the file physically on disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await supportingDocument.CopyToAsync(stream);
                Console.WriteLine("File saved successfully to disk.");
            }

            // Create the FilesModel entry and link it to the saved claim
            var fileModel = new FilesModel
            {
                FileName = uniqueFileName,
                Length = supportingDocument.Length,
                ContentType = mimeType,
                Data = System.IO.File.ReadAllBytes(filePath), // Optionally store the file data
                ClaimId = claims.ID // Link the file to the saved claim
            };

            _context.Files.Add(fileModel); // Add the file to the context
            await _context.SaveChangesAsync(); // Save the file

            Console.WriteLine("File model added to the database.");
        }
        else
        {
            claims.SupportingDocuments = ""; // If no file, assign an empty string
            Console.WriteLine("No supporting documents provided or file is empty.");
        }

        //*********************************************************************************************
        return RedirectToAction("ClaimSummary", new { id = claims.ID });
    }
    else
    {
        Console.WriteLine("ModelState is invalid"); // Log invalid model state
        foreach (var modelStateKey in ModelState.Keys)
        {
            var modelStateVal = ModelState[modelStateKey];
            foreach (var error in modelStateVal.Errors)
            {
                Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}"); // Log each error
            }
        }
    }

    return View(claims); // Return view with claim data
}

        //action to display claim submitted view
        public async Task<IActionResult> ClaimSummary(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }
            return View(claim);
        }

        public IActionResult ClaimSubmitted()
        {
            return View(); //display claim submitted view
        }

        //action to display dashboard view
        public IActionResult Dashboard()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to the login page, if not authenticated
                return RedirectToPage("/Account/Login", new { role = "Lecturer" });
            }
            return View(); //display dashboard view
        }

        //action to display admin dashboard view
        public IActionResult AdminDashboard()
        {
            return View(); //display admin dashboard view
        }

        //action to view claim history
        public async Task<IActionResult> ViewClaimHistory()
        {
            if (!User.Identity.IsAuthenticated) //Check if the user is authenticated
            {
                //Redirect to the Razor Page "/Account/Login" without parameters
                return RedirectToPage("/Account/Login");
            }
        
            try
            {
                // Get the logged-in user's ID
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"Logged in User ID: {userId}"); // Log the user ID

                // Fetch claims for the logged-in user, including related files
                var claims = await _context.Claims
                    .Include(c => c.File) // Include related files
                    .Where(c => c.UserID == userId)
                    .ToListAsync();

                if (claims == null || !claims.Any())
                {
                    Console.WriteLine("No claims found for this user.");
                }
                else
                {
                    // Log the number of claims fetched
                    Console.WriteLine($"Number of claims found: {claims.Count}");

                    // Log details of each claim for debugging
                    foreach (var claim in claims)
                    {
                        Console.WriteLine($"Claim ID: {claim.ID}, Total Files: {claim.File.Count}");

                        // Log file details
                        foreach (var file in claim.File)
                        {
                            Console.WriteLine($"File ID: {file.FileId}, File Name: {file.FileName}");
                        }
                    }
                }

                return View(claims); //Pass the claims data to the view
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return View("Error"); //Redirect to an error page if something goes wrong
            }
        }


        public async Task<IActionResult> DownloadFile(int id)
        {
            var file = await _context.Files.FirstOrDefaultAsync(f => f.FileId == id);

            if (file == null)
            {
                return NotFound();
            }

            return File(file.Data, file.ContentType, file.FileName); //Return the file for download
        }
        public async Task<IActionResult> DownloadFilee(int id)
        {
            var file = await _context.Files.FirstOrDefaultAsync(f => f.ClaimId == id);

            if (file == null)
            {
                return NotFound();
            }

            return File(file.Data, file.ContentType, file.FileName); //Return the file for download
        }


    }
}


///done part 2
