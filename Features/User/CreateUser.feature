Feature: CreateUser - Create new user

  Background:
    Given I have the "UserAPI" service initialized

  Scenario Outline: CreateUser endpoint tests
    When I create a POST request to "CreateUser" endpoint with test case "<tcNo>" query params "<queryParams>" headers "<headers>" with payload <payload>
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"
    And I verify that the field "<fieldName>" in the response is "<fieldValue>"
    And I verify the "<fieldToContain>" from the response to contain "<containsValue>"
    And I verify the "<fieldType>" type in the response to be a "<expectedType>"
    And I verify the "<fieldExists>" in the response "exists"
    And the response returned should match the schema "<schemaFile>"

    Examples:
      | tcNo  | description                         | queryParams      | headers                       | payload                  | statusCode | statusText | fieldName | fieldValue | fieldToContain | containsValue | fieldType  | expectedType | fieldExists | schemaFile              |
      | UP001 | Create user - basic                 | none             | none                          | create-user-payload.json | 201        | Created    | name      | morpheus   | name           | morph         | id         | string       | id          | create-user-schema.json |
      | UP002 | Create user - verify ID created     | none             | none                          | create-user-payload.json | 201        | Created    | id        | NotEmpty   | id             | -             | id         | string       | createdAt   | create-user-schema.json |
      | UP003 | Create user - verify job field      | none             | none                          | create-user-payload.json | 201        | Created    | job       | leader     | job            | lead          | job        | string       | job         | create-user-schema.json |
      | UP004 | Create user - with email flag       | sendEmail=true   | none                          | create-user-payload.json | 201        | Created    | name      | morpheus   | n/a            | n/a           | name       | string       | name        | create-user-schema.json |
      | UP005 | Create user - with notify param     | notify=admin     | none                          | create-user-payload.json | 201        | Created    | name      | morpheus   | createdAt      | 202           | createdAt  | string       | createdAt   | create-user-schema.json |
      | UP006 | Create user - with custom headers   | none             | Content-Type=application/json | create-user-payload.json | 201        | Created    | name      | morpheus   | n/a            | n/a           | n/a        | n/a          | id          | create-user-schema.json |
      | UP007 | Create user - verify createdAt      | none             | none                          | create-user-payload.json | 201        | Created    | createdAt | NotEmpty   | createdAt      | T             | createdAt  | string       | createdAt   | create-user-schema.json |
