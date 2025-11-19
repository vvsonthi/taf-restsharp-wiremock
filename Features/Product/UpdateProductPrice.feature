Feature: UpdateProductPrice - Partial update of product price only

  Background:
    Given I have the "ProductAPI" service initialized

  Scenario Outline: UpdateProductPrice endpoint tests
    When I create a PATCH request to "UpdateProductPrice" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>" with payload <payload>
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"

    Examples:
      | tcNo  | description                 | pathParams | queryParams | headers | payload                   | statusCode | statusText |
      | PR001 | Update price for product 101| id=101     | none        | none    | update-price-payload.json | 200        | OK         |
      | PR002 | Update price for product 102| id=102     | none        | none    | update-price-payload.json | 200        | OK         |
      | PR003 | Update price for product 103| id=103     | none        | none    | update-price-payload.json | 200        | OK         |
