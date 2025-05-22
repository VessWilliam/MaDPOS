# Retail POS System Assessment
You’ve inherited a legacy internal point-of-sale system for a retail business. It’s buggy, partially built, lacks documentation, and has several unfinished features. Your task is to:

	•	Extend the functionality
	•	Debug critical issues
	•	Build out reporting/analytics/api
	•	Deploy it online
	•	Follow Git and clean code practices

## Expected Deliverables
	1. Clean Repo
	•	Clear README guide
	•	Meaningful commit messages
	2. Postman Collection
	•	Export the API endpoints as a Postman collection or any openapi format
	•	Include the collection in the repo
    3. Hosting
    •	Host the site on any free or budget hosting platform for at least 3 days for review purpose
	•	DEPLOYMENT.md for hosting guide
    4. Final Report / Video
	•	**SHORT!!!** write-up (1–2 pages) answering:
	•	What challenges you faced?
    •	What have you done & how (dev / optimization)?
	•	What would you improve with more time?

💡 Notes
	•	You can use any .NET libraries you want
	•	You can use any CSS/JS frameworks you want
	•	You can use mssql / postgresql

## Project Structure
```
RetailPOS/
├── RetailPOS.Web/           # Main web application
├── RetailPOS.Tests/         # Unit and integration tests
└── README.md               # This file
```

## Setup Instructions
1. **Prerequisites**
   - .NET 7.0 SDK
   - Mssql
   - Visual Studio 2022 or VS Code

2. **Database Setup**
   ```bash
   dotnet ef database update
   ```

3. **Running the Application**
   ```bash
   dotnet run --project RetailPOS.Web
   ```

4. **Running Tests**
   ```bash
   dotnet test
   ```

### ✅ Evaluation Criteria

| Area           | Evaluation Criteria                                                                 |
|----------------|--------------------------------------------------------------------------------------|
| **CRUD / MVC** | Clean code, DRY practices, ViewModel usage, separation of concerns                  |
| **Reporting**  | SQL/EF Core query design, aggregation, optimization, charts integration             |
| **Debugging**  | Ability to trace errors, apply meaningful fixes, not just superficial try/catch     |
| **Code Reading** | Refactoring legacy code, making sense of unclean structure                        |
| **DevOps**     | Ability to deploy, handle configs, resolve common deployment errors                 |
| **Git Practices** | Frequent, descriptive commits; no force pushes; usage of branches/gitflow (bonus)       |
| **Crawler**    | Optional, async handling, parsing logic, retries, real-world fault tolerance                  |



## Core Challenges (Hints / Guide)
The end result is expected to be a professional and usable basic POS system.
The requirements below may seem overwhelming, ***you may choose to follow them or not.***
They are meant to serve as a guideline to help you complete the assessment, only do what you think it's important.
Feel free to use any tools, including ChatGPT — there will be no penalties.
We prioritize both speed and quality.

1. Security & Access Control
- Fix authentication guards for all pages
- Restrict user registration to admin-only (no public registration)
- Implement role-based access control (Admin, Manager, Cashier)

2. Sales Process Completion
- Create a proper and professional billing workflow:
- Cart management
- Checkout process
- Payment processing
- Receipt generation
- Implement sales status workflow (Pending → Processing → Completed/Cancelled)
- Handle stock updates during sales

3. Advanced Reporting
- Create comprehensive sales reports:
- Daily sales aggregation
- Category-wise sales analysis
- Visual representation (graphs/charts)
- Best-selling products/categories
- Sales trends over time
- Export functionality for reports

4. External API (for client facing app)
- Design and implement RESTful APIs for:
- Product catalog
- Order placement
- Order status tracking
- Stock availability
- API documentation (postman / openapi format)
- Rate limiting and security

5. OPTIONAL: Product Management Enhancement (based on your creativity)
- Implement product import functionality:
- Web scraping/crawling from competitor sites
- CSV/Excel import
- Bulk price updates
- Image import from URLs
- Price comparison features
- Stock level alerts

6. Data Integrity & Validation
Ensure data consistency across:
- Stock levels
- Sales transactions
- Price history
- User actions
- Implement proper error handling
- Data validation at all levels
