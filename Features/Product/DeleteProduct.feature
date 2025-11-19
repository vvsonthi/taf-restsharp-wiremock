Feature: DeleteProduct - Hard delete product by ID

  Background:
    Given I have the "ProductAPI" service initialized

  Scenario Outline: DeleteProduct endpoint tests
    When I create a DELETE request to "DeleteProduct" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"

    Examples:
      | tcNo  | description        | pathParams | queryParams | headers | statusCode | statusText  |
      | PD001 | Delete product 101 | id=101     | none        | none    | 204        | No Content  |
      | PD002 | Delete product 102 | id=102     | none        | none    | 204        | No Content  |
      | PD003 | Delete product 103 | id=103     | none        | none    | 204        | No Content  |
      | PD004 | Delete product 104 | id=104     | none        | none    | 204        | No Content  |
