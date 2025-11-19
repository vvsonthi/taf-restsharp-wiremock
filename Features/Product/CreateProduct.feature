Feature: CreateProduct - Create new product

  Background:
    Given I have the "ProductAPI" service initialized

  Scenario Outline: CreateProduct endpoint tests
    When I create a POST request to "CreateProduct" endpoint with test case "<tcNo>" query params "<queryParams>" headers "<headers>" with payload <payload>
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"

    Examples:
      | tcNo  | description                    | queryParams | headers | payload                     | statusCode | statusText |
      | PP001 | Create laptop product          | none        | none    | create-product-payload.json | 201        | Created    |
      | PP002 | Create product with notify     | notify=admin| none    | create-product-payload.json | 201        | Created    |
      | PP003 | Create product with headers    | none        | Content-Type=application/json | create-product-payload.json | 201 | Created |
