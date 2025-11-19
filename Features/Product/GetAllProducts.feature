Feature: GetAllProducts - Retrieve list of all products with pagination

  Background:
    Given I have the "ProductAPI" service initialized

  Scenario Outline: GetAllProducts endpoint tests
    When I create a GET request to "GetAllProducts" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"

    Examples:
      | tcNo  | description                        | pathParams | queryParams      | headers | statusCode | statusText |
      | PA001 | Get all products - page 1          | none       | page=1&limit=10  | none    | 200        | OK         |
      | PA002 | Get all products - page 2          | none       | page=2&limit=10  | none    | 200        | OK         |
      | PA003 | Get all products - page 1 limit 20 | none       | page=1&limit=20  | none    | 200        | OK         |
      | PA004 | Get all products - page 3          | none       | page=3&limit=10  | none    | 200        | OK         |
      | PA005 | Get all products - custom limit    | none       | page=1&limit=5   | none    | 200        | OK         |
