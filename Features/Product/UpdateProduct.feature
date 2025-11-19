Feature: UpdateProduct - Full update of existing product

  Background:
    Given I have the "ProductAPI" service initialized

  Scenario Outline: UpdateProduct endpoint tests
    When I create a PUT request to "UpdateProduct" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>" with payload <payload>
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"

    Examples:
      | tcNo  | description              | pathParams | queryParams | headers | payload                     | statusCode | statusText |
      | PU001 | Update product 101       | id=101     | none        | none    | update-product-payload.json | 200        | OK         |
      | PU002 | Update product 102       | id=102     | none        | none    | update-product-payload.json | 200        | OK         |
      | PU003 | Update product 103       | id=103     | none        | none    | update-product-payload.json | 200        | OK         |
