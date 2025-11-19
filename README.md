# TAF RestSharp WireMock - API Test Automation Framework

A comprehensive **BDD-based API Test Automation Framework** built with **C#**, **RestSharp**, **Reqnroll**, and **WireMock.NET**.

## Overview

This framework provides multiple testing patterns for REST API automation, from simple inline parameters to sophisticated YAML-based service configurations. Choose the pattern that best fits your needs.

## Quick Start

### Recommended Pattern: Endpoint-Based Organization

The framework uses **endpoint-based organization** where **1 feature file = 1 endpoint = all test cases**. Each endpoint has its own feature file with all test variations in a comprehensive Examples table.

```gherkin
Feature: GetUserById - Retrieve specific user by ID

  Background:
    Given I have the "UserAPI" service initialized

  Scenario Outline: GetUserById endpoint tests
    When I create a GET request to "GetUserById" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"
    And I verify that the field "<fieldName>" in the response is "<fieldValue>"

    Examples:
      | tcNo  | description                | pathParams | queryParams | headers | statusCode | statusText | fieldName | fieldValue |
      | UG001 | Get user 2 - verify ID     | id=2       | none        | none    | 200        | OK         | data.id   | 2          |
      | UG002 | Get user 3 - verify ID     | id=3       | none        | none    | 200        | OK         | data.id   | 3          |
      | UG007 | Get user 999 - not found   | id=999     | none        | none    | 404        | Not Found  | n/a       | n/a        |
```

**Key Principles:**
- ✅ One feature file per endpoint
- ✅ File name matches endpoint name (e.g., `User_GetUserById.feature`)
- ✅ All test cases in one comprehensive Examples table
- ✅ Description column for context

See [Endpoint-Based Organization Guide](ENDPOINT_BASED_ORGANIZATION_GUIDE.md) for complete documentation.

## Architecture

### Project Structure

```
taf-restsharp-wiremock/
├── Config/
│   └── ApiServices/              # YAML API service configurations
│       ├── UserAPI.yaml
│       └── ProductAPI.yaml
├── Data/
│   ├── Payloads/                 # JSON request payloads (organized by API)
│   │   ├── User/
│   │   │   ├── create-user-payload.json
│   │   │   ├── update-user-payload.json
│   │   │   └── patch-user-payload.json
│   │   └── Product/
│   │       ├── create-product-payload.json
│   │       ├── update-product-payload.json
│   │       ├── update-price-payload.json
│   │       └── update-stock-payload.json
│   └── QueryParams/              # Query parameter files (organized by API)
│       ├── User/
│       │   ├── user-query-params.txt
│       │   ├── post-query-params.txt
│       │   ├── update-query-params.txt
│       │   └── delete-query-params.txt
│       └── Product/
├── Features/                     # Gherkin feature files (organized by API)
│   ├── User/                     # User API endpoints
│   │   ├── GetAllUsers.feature
│   │   ├── GetUserById.feature
│   │   ├── CreateUser.feature
│   │   ├── UpdateUser.feature
│   │   ├── PartialUpdateUser.feature
│   │   └── DeleteUser.feature
│   ├── Product/                  # Product API endpoints
│   │   ├── GetAllProducts.feature
│   │   ├── GetProductById.feature
│   │   ├── GetProductsByCategory.feature
│   │   ├── SearchProducts.feature
│   │   ├── CreateProduct.feature
│   │   ├── UpdateProduct.feature
│   │   ├── UpdateProductPrice.feature
│   │   ├── UpdateProductStock.feature
│   │   ├── DeleteProduct.feature
│   │   └── ArchiveProduct.feature
│   └── SchemaValidation.feature  # Schema validation examples
├── Schemas/                      # JSON Schema files (organized by API)
│   ├── User/                     # User API schemas
│   │   ├── get-user-by-id-schema.json
│   │   ├── get-all-users-schema.json
│   │   ├── create-user-schema.json
│   │   └── update-user-schema.json
│   └── Product/                  # Product API schemas
│       ├── get-product-by-id-schema.json
│       ├── get-all-products-schema.json
│       └── create-product-schema.json
├── StepDefinitions/              # Step definition classes
│   ├── ApiServiceSteps.cs        # API Service Pattern steps
│   ├── UnifiedRestSteps.cs       # Unified Pattern steps
│   └── ValidationSteps.cs        # 8 comprehensive validation steps
├── Utils/
│   ├── Api/                      # API client wrapper
│   ├── ApiServices/              # API service loaders/managers
│   ├── Configuration/            # Config management
│   ├── Headers/                  # Header parsing utilities
│   ├── Json/                     # JSON utilities
│   ├── QueryParams/              # Query param loaders
│   └── Validation/               # Field validators
├── Models/                       # POCOs and config models
└── appsettings.json             # Environment configurations
```

### Core Components

#### 1. ApiClient (Utils/Api/ApiClient.cs)
- RestSharp wrapper for HTTP operations
- Supports GET, POST, PUT, PATCH, DELETE
- Configurable timeout and base URL
- Automatic header management

#### 2. ApiServiceManager (Utils/ApiServices/ApiServiceManager.cs)
- Manages API service context per scenario
- Loads YAML configurations
- Provides endpoint URL building
- Handles environment-specific overrides

#### 3. ConfigurationManager (Utils/Configuration/ConfigurationManager.cs)
- Singleton for appsettings.json access
- Multi-environment support (dev, test, qa, staging, prod)
- WireMock configuration per environment

#### 4. QueryParamLoader (Utils/QueryParams/QueryParamLoader.cs)
- Auto-detection: files (.txt) vs inline (param=value&param2=value2)
- TC-based parameter loading from files
- Supports "none" keyword

#### 5. FieldValidator (Utils/Validation/FieldValidator.cs)
- Response field validation with special values
- Supports: NotEmpty, Empty, Null, NotNull, Any
- Nested field access: `data.user.email`
- Array indexing: `users[0].name`

## Testing Patterns

### Pattern 1: API Service Pattern (Recommended)

**Best for:** All scenarios, especially when testing multiple APIs with many endpoints

**Advantages:**
- Cleanest Gherkin (no hardcoded URLs)
- Centralized endpoint configuration
- Environment-aware (auto-applies env overrides)
- Supports path parameters, timeouts, default headers per API
- Easy to maintain (change URL once, all tests update)

**Example:**
```gherkin
Given I have the "UserAPI" service initialized
When I create a POST request to "CreateUser" endpoint with test case "TC001" query params "none" headers "none" with payload create-user-payload.json
Then I receive a response with HTTP status code "201" with status text "Created"
```

**Configuration (Config/ApiServices/UserAPI.yaml):**
```yaml
apiName: UserAPI
baseUrl: https://reqres.in
timeout: 8000

environments:
  dev:
    baseUrl: https://dev-reqres.in

endpoints:
  CreateUser:
    path: /api/users
    method: POST

defaultHeaders:
  Accept: application/json
```

**Documentation:** [API_SERVICE_PATTERN_GUIDE.md](API_SERVICE_PATTERN_GUIDE.md)

---

### Pattern 2: Unified Pattern

**Best for:** Projects where endpoints change frequently or when you want full control in Gherkin

**Advantages:**
- All parameters visible in Gherkin
- Flexible inline or file-based parameters
- Supports "none" keyword for optional parameters

**Example:**
```gherkin
When I create a GET request with test case "TC001" uri params "users/2" query params "page=1&per_page=5" headers "Accept=application/json" with payload none
Then I receive a response with HTTP status code "200" with status text "OK"
```

**Documentation:** [UNIFIED_PATTERN_GUIDE.md](UNIFIED_PATTERN_GUIDE.md)

---

### Pattern 3: Inline vs File-Based Parameters

**Decision Tree:**
- **Inline:** Use for 1-3 simple parameters (e.g., `page=1&limit=10`)
- **File:** Use for 4+ parameters or when reusing across scenarios

**Examples:**

**Inline Query Params:**
```gherkin
When I create a GET request to "GetAllUsers" endpoint with test case "TC001" query params "page=1&per_page=5" headers "none"
```

**File-Based Query Params:**
```gherkin
When I create a GET request to "GetAllUsers" endpoint with test case "TC001" query params "user-query-params.txt" headers "none"
```

**File Format (Data/QueryParams/user-query-params.txt):**
```
TC001: page=1
TC002: page=2&per_page=10
TC003: page=1&per_page=5&sort=asc
```

**Documentation:** [INLINE_VS_FILE_GUIDE.md](INLINE_VS_FILE_GUIDE.md)

## Configuration

### Multi-Environment Support

**appsettings.json:**
```json
{
  "env": "dev",
  "UseWireMock": true,
  "WireMock": {
    "Port": 9091,
    "AdminPort": 9092,
    "EnableAdmin": true
  },
  "dev": {
    "ApiKey": "DEV-12345",
    "UseWireMock": true,
    "WireMock": {
      "Port": 9091,
      "AdminPort": 9092,
      "EnableAdmin": true
    }
  },
  "prod": {
    "ApiKey": "PROD-SECRET-KEY",
    "UseWireMock": false
  }
}
```

### Environment-Specific API Configuration

YAML files support environment-specific overrides:

```yaml
baseUrl: https://reqres.in  # Default
timeout: 8000

environments:
  dev:
    baseUrl: https://dev-reqres.in
    timeout: 10000

  prod:
    baseUrl: https://reqres.in
    timeout: 15000
```

The framework automatically applies overrides based on `appsettings.json` → `env` setting.

## Common Scenarios

### CRUD Operations

```gherkin
Feature: Complete CRUD Workflow

  Background:
    Given I have the "UserAPI" service initialized

  Scenario: Complete user lifecycle
    # Create
    When I create a POST request to "CreateUser" endpoint with test case "TC001" query params "none" headers "none" with payload create-user-payload.json
    Then I receive a response with HTTP status code "201" with status text "Created"

    # Read
    When I create a GET request to "GetUserById" endpoint with path params "id=2" test case "TC001" query params "none" headers "none"
    Then I receive a response with HTTP status code "200" with status text "OK"

    # Update
    When I create a PUT request to "UpdateUser" endpoint with path params "id=2" test case "TC001" query params "none" headers "none" with payload update-user-payload.json
    Then I receive a response with HTTP status code "200" with status text "OK"

    # Delete
    When I create a DELETE request to "DeleteUser" endpoint with path params "id=2" test case "TC001" query params "none" headers "none"
    Then I receive a response with HTTP status code "204" with status text "No Content"
```

### Data-Driven Testing

```gherkin
Scenario Outline: Get user by ID
  When I create a GET request to "GetUserById" endpoint with path params "id=<userId>" test case "<tcNo>" query params "none" headers "none"
  Then I receive a response with HTTP status code "200" with status text "OK"
  And I verify that the field "data.id" in the response is "<userId>"

  Examples:
    | tcNo  | userId |
    | TC001 | 2      |
    | TC002 | 3      |
    | TC003 | 4      |
```

### Field Validation

```gherkin
Then I verify that the field "data.id" in the response is "2"
And I verify that the field "data.email" in the response is "NotEmpty"
And I verify that the field "data.first_name" in the response is "Janet"
And I verify that the field "support" in the response is "NotNull"
```

**Special Values:**
- `NotEmpty` - Field exists and is not empty
- `Empty` - Field exists and is empty
- `Null` - Field is null
- `NotNull` - Field is not null
- `Any` - Field exists (any value)

### Header Validation

```gherkin
Then I confirm that the expected headers "Content-Type=application/json,X-Custom-Header=value" are present
```

### Multiple APIs in One Feature

```gherkin
Feature: Cross-API Testing

  Scenario: Test User API
    Given I have the "UserAPI" service initialized
    When I create a GET request to "GetAllUsers" endpoint with test case "TC001" query params "none" headers "none"
    Then I receive a response with HTTP status code "200" with status text "OK"

  Scenario: Test Product API
    Given I have the "ProductAPI" service initialized
    When I create a GET request to "GetAllProducts" endpoint with test case "TC001" query params "none" headers "none"
    Then I receive a response with HTTP status code "200" with status text "OK"
```

## Test Data Management

### Payloads (Data/Payloads/)

**create-user-payload.json:**
```json
{
  "name": "morpheus",
  "job": "leader"
}
```

Reference in Gherkin:
```gherkin
with payload create-user-payload.json
```

### Query Parameters (Data/QueryParams/)

**user-query-params.txt:**
```
TC001: page=1
TC002: page=2
TC003: page=1&per_page=5
```

Reference in Gherkin:
```gherkin
query params "user-query-params.txt"
```

## Running Tests

### Command Line

```bash
# Run all tests
dotnet test

# Run specific feature
dotnet test --filter "DisplayName~UserAPI"

# Run with specific environment
# (Set "env" in appsettings.json first)
dotnet test
```

### Visual Studio

1. Open Test Explorer
2. Right-click feature/scenario
3. Click "Run"

## WireMock Integration

WireMock.NET is configured per environment in `appsettings.json`.

**Enable WireMock:**
```json
{
  "env": "dev",
  "UseWireMock": true,
  "WireMock": {
    "Port": 9091,
    "AdminPort": 9092,
    "EnableAdmin": true
  }
}
```

**Access WireMock Admin UI:**
```
http://localhost:9092/__admin/
```

## Key Features

- **Multiple Testing Patterns** - Choose what fits your needs
- **8 Comprehensive Validation Steps** - Field exact/contains, headers, existence, type, schema validation
- **JSON Schema Validation** - Complete response structure verification
- **Environment-Aware** - Dev, Test, QA, Staging, Prod configurations
- **Data-Driven** - Scenario Outline with Examples tables
- **Flexible Parameters** - Inline or file-based
- **Path Parameters** - Dynamic URL building (`/api/users/{id}`)
- **Advanced Field Validation** - Nested fields, arrays, special values, type checking
- **Header Management** - Default + custom headers, presence and value verification
- **YAML Configuration** - Clean API service definitions
- **WireMock Support** - API mocking per environment
- **Reusable Components** - Singleton pattern, utilities
- **Schema Organization** - API-specific schema folders

## Documentation

| Guide | Description |
|-------|-------------|
| [ENDPOINT_BASED_ORGANIZATION_GUIDE.md](ENDPOINT_BASED_ORGANIZATION_GUIDE.md) | **1 file per endpoint organization** - How to structure tests - **Start Here!** |
| [ADVANCED_VALIDATION_GUIDE.md](ADVANCED_VALIDATION_GUIDE.md) | **8 comprehensive validation steps** - Complete response verification guide |
| [API_SERVICE_PATTERN_GUIDE.md](API_SERVICE_PATTERN_GUIDE.md) | Complete guide for API Service Pattern (YAML-based) |
| [DATA_DRIVEN_PATTERN_GUIDE.md](DATA_DRIVEN_PATTERN_GUIDE.md) | Comprehensive Examples tables - Eliminate redundancy |
| [UNIFIED_PATTERN_GUIDE.md](UNIFIED_PATTERN_GUIDE.md) | Unified REST pattern with all parameters in Gherkin |
| [INLINE_VS_FILE_GUIDE.md](INLINE_VS_FILE_GUIDE.md) | Decision tree for inline vs file-based parameters |
| [QUERY_PARAMS_GUIDE.md](QUERY_PARAMS_GUIDE.md) | Query parameter management guide |
| [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md) | Initial implementation summary |

## Best Practices

### 1. Use API Service Pattern for New Tests

Start with the API Service Pattern. It provides the cleanest Gherkin and best maintainability.

### 2. Logical Endpoint Names

Use descriptive, action-oriented names:
- GetAllUsers
- GetUserById
- CreateUser
- UpdateUser
- DeleteUser

### 3. TC-Based Test Data

Organize test data by test case numbers:
```
TC001: scenario description
TC002: edge case
TC003: negative test
```

### 4. Environment-Specific Configs

Always provide environment overrides in YAML:
```yaml
environments:
  dev: ...
  test: ...
  qa: ...
  staging: ...
  prod: ...
```

### 5. Descriptive Scenario Names

```gherkin
# Good
Scenario: Get user by ID returns correct user data

# Bad
Scenario: Test 1
```

### 6. Use "none" for Optional Parameters

```gherkin
query params "none"
headers "none"
with payload none
```

## Tech Stack

- **Language:** C# (.NET)
- **BDD Framework:** Reqnroll (SpecFlow successor)
- **HTTP Client:** RestSharp
- **Mocking:** WireMock.NET
- **Configuration:** YamlDotNet, Microsoft.Extensions.Configuration
- **Testing:** MSTest
- **JSON:** Newtonsoft.Json

## Project History

This framework evolved through several iterations:

1. **Initial Setup** - Basic CRUD operations with hardcoded endpoints
2. **Query Params Enhancement** - TC-based external parameter files
3. **Unified Pattern** - Single step definition for all HTTP methods
4. **Inline Parameters** - Auto-detection of inline vs file-based params
5. **API Service Pattern** - YAML-based endpoint externalization (current)

## Contributing

When adding new features:

1. Create YAML configuration for new APIs (Config/ApiServices/)
2. Add example feature files demonstrating usage
3. Update relevant documentation guides
4. Follow existing naming conventions
5. Add validation steps for assertions

## Troubleshooting

### Error: API service file not found

**Solution:** Ensure `Config/ApiServices/<ApiName>.yaml` exists and name matches exactly (case-sensitive).

### Error: Endpoint not found

**Solution:** Check endpoint name in Gherkin matches YAML exactly.

### Error: No API service initialized

**Solution:** Add `Given I have the "<ApiName>" service initialized` before When steps.

### Error: Path parameter not replaced

**Solution:** Use path params step: `path params "id=2"`

## License

[Your License Here]

## Contact

[Your Contact Info Here]
