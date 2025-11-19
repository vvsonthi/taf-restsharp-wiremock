Feature: UpdateUser - Full update of existing user

  Background:
    Given I have the "UserAPI" service initialized

  Scenario Outline: UpdateUser endpoint tests
    When I create a PUT request to "UpdateUser" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>" with payload <payload>
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"
    And I verify that the field "<fieldName>" in the response is "<fieldValue>"

    Examples:
      | tcNo  | description                       | pathParams | queryParams | headers | payload                  | statusCode | statusText | fieldName | fieldValue |
      | UU001 | Update user 2 - verify timestamp  | id=2       | none        | none    | update-user-payload.json | 200        | OK         | updatedAt | NotEmpty   |
      | UU002 | Update user 3 - verify timestamp  | id=3       | none        | none    | update-user-payload.json | 200        | OK         | updatedAt | NotEmpty   |
      | UU003 | Update user 2 - verify name       | id=2       | none        | none    | update-user-payload.json | 200        | OK         | name      | morpheus   |
      | UU004 | Update user 2 - verify job        | id=2       | none        | none    | update-user-payload.json | 200        | OK         | job       | zion resident |
