Feature: GetAllUsers - Retrieve list of users with pagination

  Background:
    Given I have the "UserAPI" service initialized

  Scenario Outline: GetAllUsers endpoint tests
    When I create a GET request to "GetAllUsers" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"
    And I verify that the field "<fieldName>" in the response is "<fieldValue>"

    Examples:
      | tcNo  | description                           | pathParams | queryParams           | headers | statusCode | statusText | fieldName | fieldValue |
      | UA001 | Get all users - first page            | none       | page=1                | none    | 200        | OK         | data      | NotEmpty   |
      | UA002 | Get all users - second page           | none       | page=2                | none    | 200        | OK         | data      | NotEmpty   |
      | UA003 | Get all users - custom per_page       | none       | page=1&per_page=5     | none    | 200        | OK         | per_page  | 5          |
      | UA004 | Get all users - page 1 verify page    | none       | page=1                | none    | 200        | OK         | page      | 1          |
      | UA005 | Get all users - page 2 verify page    | none       | page=2                | none    | 200        | OK         | page      | 2          |
      | UA006 | Get all users - verify data structure | none       | page=1                | none    | 200        | OK         | total     | NotEmpty   |
      | UA007 | Get all users - from query param file | none       | user-query-params.txt | none    | 200        | OK         | data      | NotEmpty   |
      | UA008 | Get all users - with custom headers   | none       | page=1                | Accept=application/json | 200 | OK    | data      | NotEmpty   |
