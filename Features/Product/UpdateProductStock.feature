Feature: UpdateProductStock - Partial update of product stock only

  Background:
    Given I have the "ProductAPI" service initialized

  Scenario Outline: UpdateProductStock endpoint tests
    When I create a PATCH request to "UpdateProductStock" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>" with payload <payload>
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"

    Examples:
      | tcNo  | description                  | pathParams | queryParams | headers | payload                    | statusCode | statusText |
      | PK001 | Update stock for product 101 | id=101     | none        | none    | update-stock-payload.json  | 200        | OK         |
      | PK002 | Update stock for product 102 | id=102     | none        | none    | update-stock-payload.json  | 200        | OK         |
      | PK003 | Update stock for product 103 | id=103     | none        | none    | update-stock-payload.json  | 200        | OK         |
