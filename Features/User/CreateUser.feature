Feature: CreateUser - Create new user

  Background:
    Given I have the "UserAPI" service initialized

  Scenario Outline: CreateUser endpoint tests
    When I create a POST request to "CreateUser" endpoint with test case "<tcNo>" query params "<queryParams>" headers "<headers>" with payload <payload>
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"
    And I verify that the field "<fieldName>" in the response is "<fieldValue>"

    Examples:
      | tcNo  | description                         | queryParams      | headers                      | payload                  | statusCode | statusText | fieldName | fieldValue |
      | UP001 | Create user - basic                 | none             | none                         | create-user-payload.json | 201        | Created    | name      | morpheus   |
      | UP002 | Create user - verify ID created     | none             | none                         | create-user-payload.json | 201        | Created    | id        | NotEmpty   |
      | UP003 | Create user - verify job field      | none             | none                         | create-user-payload.json | 201        | Created    | job       | leader     |
      | UP004 | Create user - with email flag       | sendEmail=true   | none                         | create-user-payload.json | 201        | Created    | name      | morpheus   |
      | UP005 | Create user - with notify param     | notify=admin     | none                         | create-user-payload.json | 201        | Created    | name      | morpheus   |
      | UP006 | Create user - with custom headers   | none             | Content-Type=application/json| create-user-payload.json | 201        | Created    | name      | morpheus   |
      | UP007 | Create user - verify createdAt      | none             | none                         | create-user-payload.json | 201        | Created    | createdAt | NotEmpty   |
