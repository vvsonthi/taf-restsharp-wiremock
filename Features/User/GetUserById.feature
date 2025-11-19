Feature: GetUserById - Retrieve specific user by ID

  Background:
    Given I have the "UserAPI" service initialized

  Scenario Outline: GetUserById endpoint tests
    When I create a GET request to "GetUserById" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"
    And I verify that the field "<fieldName>" in the response is "<fieldValue>"

    Examples:
      | tcNo  | description                      | pathParams | queryParams | headers | statusCode | statusText | fieldName       | fieldValue |
      | UG001 | Get user 2 - verify ID           | id=2       | none        | none    | 200        | OK         | data.id         | 2          |
      | UG002 | Get user 3 - verify ID           | id=3       | none        | none    | 200        | OK         | data.id         | 3          |
      | UG003 | Get user 4 - verify ID           | id=4       | none        | none    | 200        | OK         | data.id         | 4          |
      | UG004 | Get user 2 - verify email exists | id=2       | none        | none    | 200        | OK         | data.email      | NotEmpty   |
      | UG005 | Get user 2 - verify first name   | id=2       | none        | none    | 200        | OK         | data.first_name | Janet      |
      | UG006 | Get user 3 - verify first name   | id=3       | none        | none    | 200        | OK         | data.first_name | Emma       |
      | UG007 | Get user 999 - not found         | id=999     | none        | none    | 404        | Not Found  | n/a             | n/a        |
