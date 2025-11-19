Feature: DeleteUser - Delete user by ID

  Background:
    Given I have the "UserAPI" service initialized

  Scenario Outline: DeleteUser endpoint tests
    When I create a DELETE request to "DeleteUser" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"

    Examples:
      | tcNo  | description           | pathParams | queryParams | headers | statusCode | statusText  |
      | UD001 | Delete user 2         | id=2       | none        | none    | 204        | No Content  |
      | UD002 | Delete user 3         | id=3       | none        | none    | 204        | No Content  |
      | UD003 | Delete user 4         | id=4       | none        | none    | 204        | No Content  |
      | UD004 | Delete user 5         | id=5       | none        | none    | 204        | No Content  |
