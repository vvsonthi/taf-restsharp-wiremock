Feature: GetProductById - Retrieve specific product by ID

  Background:
    Given I have the "ProductAPI" service initialized

  Scenario Outline: GetProductById endpoint tests
    When I create a GET request to "GetProductById" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"

    Examples:
      | tcNo  | description           | pathParams | queryParams | headers | statusCode | statusText |
      | PG001 | Get product 101       | id=101     | none        | none    | 200        | OK         |
      | PG002 | Get product 102       | id=102     | none        | none    | 200        | OK         |
      | PG003 | Get product 103       | id=103     | none        | none    | 200        | OK         |
      | PG004 | Get product 104       | id=104     | none        | none    | 200        | OK         |
      | PG005 | Get product 999 - not found | id=999 | none        | none    | 404        | Not Found  |
