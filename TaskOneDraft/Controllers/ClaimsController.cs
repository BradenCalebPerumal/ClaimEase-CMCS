using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
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
        public IActionResult GenerateReportt()
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
 // Instantiate the validator
    var validator = new ClaimsValidator();
    var validationResult = validator.Validate(claims);
            ModelState.Remove("ClaimStatus");
            // Set the claim status based on validation
            if (!validationResult.IsValid)
    {
        claims.ClaimStatus = "Rejected";
        Console.WriteLine("Claim rejected due to rule violations.");
    }
    else
    {
        claims.ClaimStatus = "Pending";
    }    
    ModelState.Remove("DateSubmitted");
            ModelState.Remove("RateHour");
            ModelState.Remove("HoursWorked");
            ModelState.Remove("OvertimeHours");
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
        Console.WriteLine("ModelState is invalidd"); // Log invalid model state
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


        [HttpPost]
        public IActionResult GenerateReportt(DateTime startDate, DateTime endDate)
        {
            // Fetch all claims within the date range
            var claims = _context.Claims
                .Where(c => c.DateSubmitted >= startDate && c.DateSubmitted <= endDate)
                .ToList();

            if (!claims.Any())
            {
                TempData["ErrorMessage"] = "No claims found within the specified date range.";
                return RedirectToAction("ViewLecturers");
            }

            // Separate claims by status
            var approvedClaims = claims.Where(c => c.ClaimStatus == "Approved").ToList();
            var otherStatusClaims = claims.Where(c => c.ClaimStatus != "Approved").ToList();

            // Generate PDF report
            var pdfBytes = GeneratePdf(approvedClaims, otherStatusClaims, startDate, endDate);

            // Return the PDF file
            return File(pdfBytes, "application/pdf", $"ClaimReport_{startDate:yyyyMMdd}-{endDate:yyyyMMdd}.pdf");
        }
        private byte[] GeneratePdf(
    IEnumerable<Claims> approvedClaims,
    IEnumerable<Claims> otherStatusClaims,
    DateTime startDate,
    DateTime endDate)
        {
            // Calculate totals for approved claims
            var totalGrossPay = approvedClaims.Sum(c => c.TotalAmount);
            var totalTaxDeductions = approvedClaims.Sum(c => CalculateTax(c.TotalAmount));
            var totalNetPay = totalGrossPay - totalTaxDeductions;
            var totalOvertimeHours = approvedClaims.Sum(c => c.OvertimeHours ?? 0);
            var totalHoursWorked = approvedClaims.Sum(c => c.HoursWorked);

            using (var memoryStream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 40, 40, 40, 40); // Margins
                var writer = PdfWriter.GetInstance(document, memoryStream);
              

                document.Open();
                PdfContentByte cb = writer.DirectContent;
                // Define the rectangle for the border
                Rectangle pageBorder = new Rectangle(
                    document.LeftMargin - 10,
                    document.BottomMargin - 10,
                    document.PageSize.Width - document.RightMargin + 10,
                    document.PageSize.Height - document.TopMargin + 10
                );

                // Set the border style
                pageBorder.BorderWidth = 3f; // Bold border
                pageBorder.BorderColor = BaseColor.BLACK;
                pageBorder.Border = Rectangle.BOX;

                // Add the border to the page
                cb.Rectangle(pageBorder);
                // Create a table to hold the logo and header text side by side
                var headerTable = new PdfPTable(2) { WidthPercentage = 100 };
                headerTable.SetWidths(new float[] { 1f, 3f }); // Adjust column widths

                // Add Logo Cell
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Logo1.png");
               
                    var logo = Image.GetInstance(logoPath);
                    logo.ScaleToFit(43f, 43f); // Adjust size for better proportion
                    logo.Alignment = Element.ALIGN_CENTER;

                    var logoCell = new PdfPCell(logo)
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                            PaddingLeft = 120f // Add some padding for spacing

                    };
                    headerTable.AddCell(logoCell);
               

                // Add Company Name and Details with Styling
                var headerFont = FontFactory.GetFont("Arial", 18, Font.BOLD, BaseColor.BLACK);
                var detailsFont = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.GRAY);

                var headerText = new Paragraph();
                headerText.Add(new Chunk("ClaimEase CMCS Pty(Ltd)\n", headerFont));
                headerText.Add(new Chunk("Phone: 0861 555 181\n", detailsFont));
                headerText.Add(new Chunk("Email: support@cmcs.com\n", detailsFont));
                headerText.Add(new Chunk("Address: 138 Athens Street, Durban, South Africa", detailsFont));

                var textCell = new PdfPCell(headerText)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingLeft = 50f // Add some padding for spacing
                };
                headerTable.AddCell(textCell);

                // Add the header table to the document
                document.Add(headerTable);

                // Add a line separator below the header for a cleaner look
                var separator = new LineSeparator(1f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2);
                document.Add(separator);

                // Add some spacing below the header

                // Add Main Title
                var mainTitleFont = FontFactory.GetFont("Arial", 22, Font.BOLD, BaseColor.BLACK);
                document.Add(new Paragraph("Claim Report", mainTitleFont));
              
                // Add Date Range Subheading
                var subHeaderFont = FontFactory.GetFont("Arial", 12, Font.ITALIC, BaseColor.DARK_GRAY);
                document.Add(new Paragraph($"Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}", subHeaderFont));

                // Add another space for separation
                document.Add(new Paragraph(" "));

                var separatorr = new LineSeparator(1f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2);
                document.Add(separator);

                // Add Table for Claims with Other Statuses
                if (otherStatusClaims.Any())
                {
                    var otherClaimsTitleFont = FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.RED);
                    document.Add(new Paragraph("Unapproved Claims", otherClaimsTitleFont));
                    document.Add(new Paragraph(" "));

                    var otherStatusTable = new PdfPTable(5) { WidthPercentage = 100 };
                    otherStatusTable.SpacingBefore = 10f;
                    otherStatusTable.SpacingAfter = 10f;

                    // Table Header
                    AddTableHeader(otherStatusTable, new[] { "Claim ID", "Lecturer ID", "Status", "Gross Pay (R)", "Date Submitted" });

                    if (otherStatusClaims.Any())
                    {
                        foreach (var claim in otherStatusClaims)
                        {
                            otherStatusTable.AddCell(claim.ID.ToString());
                            otherStatusTable.AddCell(claim.LecturerID);
                            otherStatusTable.AddCell(claim.ClaimStatus);
                            otherStatusTable.AddCell(claim.TotalAmount.ToString("F2"));
                            otherStatusTable.AddCell(claim.DateSubmitted.ToString("yyyy-MM-dd"));
                        }
                    }
                    else
                    {
                        var noClaimsCell = new PdfPCell(new Phrase("No Unapproved Claims", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.GRAY)))
                        {
                            Colspan = 5,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        otherStatusTable.AddCell(noClaimsCell);
                    }


                    document.Add(otherStatusTable);
                }

                // Add Table for Approved Claims
                var approvedClaimsTitleFont = FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.GREEN);
                document.Add(new Paragraph("Approved Claims", approvedClaimsTitleFont));
                document.Add(new Paragraph(" "));

                var approvedTable = new PdfPTable(6) { WidthPercentage = 100 };
                approvedTable.SpacingBefore = 10f;
                approvedTable.SpacingAfter = 10f;

                // Table Header
                AddTableHeader(approvedTable, new[] { "Claim ID", "Lecturer ID", "Hours Worked", "Overtime Hours", "Gross Pay (R)", "Net Pay (R)" });

                if (approvedClaims.Any())
                {
                    foreach (var claim in approvedClaims)
                    {
                        approvedTable.AddCell(claim.ID.ToString());
                        approvedTable.AddCell(claim.LecturerID);
                        approvedTable.AddCell(claim.HoursWorked.ToString("F2"));
                        approvedTable.AddCell((claim.OvertimeHours ?? 0).ToString("F2"));
                        approvedTable.AddCell(claim.TotalAmount.ToString("F2"));
                        approvedTable.AddCell((claim.TotalAmount - CalculateTax(claim.TotalAmount)).ToString("F2"));
                    }
                }
                else
                {
                    var noClaimsCell = new PdfPCell(new Phrase("No Approved Claims", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.GRAY)))
                    {
                        Colspan = 6,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    approvedTable.AddCell(noClaimsCell);
                }

                document.Add(approvedTable);

                // Add Summary Table for Approved Claims
                document.Add(new Paragraph(" "));
                var summaryTitleFont = FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.DARK_GRAY);
                document.Add(new Paragraph("Summary for Approved Claims", summaryTitleFont));
                document.Add(new Paragraph(" "));

                var summaryTable = new PdfPTable(2) { WidthPercentage = 50, HorizontalAlignment = Element.ALIGN_LEFT };
                summaryTable.SpacingBefore = 10f;
                summaryTable.SpacingAfter = 10f;

                // Add Header Row
                AddTableHeader(summaryTable, new[] { "Description", "Value" });

                // Add Data Rows
                summaryTable.AddCell("Total Hours Worked");
                summaryTable.AddCell($"{totalHoursWorked:F2}");

                summaryTable.AddCell("Total Overtime Hours");
                summaryTable.AddCell($"{totalOvertimeHours:F2}");

                summaryTable.AddCell("Total Gross Pay");
                summaryTable.AddCell($"R{totalGrossPay:F2}");

                summaryTable.AddCell("Total Tax Deductions");
                summaryTable.AddCell($"R{totalTaxDeductions:F2}");

                summaryTable.AddCell("Total Net Pay");
                summaryTable.AddCell($"R{totalNetPay:F2}");

                document.Add(summaryTable);

                document.Close();
                return memoryStream.ToArray();
            }
        }

        private void AddTableHeader(PdfPTable table, string[] headers)
        {
            foreach (var header in headers)
            {
                var cell = new PdfPCell(new Phrase(header, FontFactory.GetFont("Arial", 12, Font.BOLD, BaseColor.WHITE)))
                {
                    BackgroundColor = BaseColor.DARK_GRAY,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                table.AddCell(cell);
            }
        }

        private double CalculateTax(double taxableIncome)
        {
            if (taxableIncome <= 237100)
                return taxableIncome * 0.18;
            else if (taxableIncome <= 370500)
                return 42678 + (taxableIncome - 237100) * 0.26;
            else if (taxableIncome <= 512800)
                return 77362 + (taxableIncome - 370500) * 0.31;
            else if (taxableIncome <= 673000)
                return 121475 + (taxableIncome - 512800) * 0.36;
            else if (taxableIncome <= 857900)
                return 179147 + (taxableIncome - 673000) * 0.39;
            else if (taxableIncome <= 1817000)
                return 251258 + (taxableIncome - 857900) * 0.41;
            else
                return 644489 + (taxableIncome - 1817000) * 0.45;
        }
        [HttpGet]
        public IActionResult GenerateReportForm(string lecturerId)
        {
            ViewBag.LecturerId = lecturerId;
            return View();
        }

    }
}


///done part 3
