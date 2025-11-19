Feature: GetProductsByCategory - Retrieve products filtered by category

  Background:
    Given I have the "ProductAPI" service initialized

  Scenario Outline: GetProductsByCategory endpoint tests
    When I create a GET request to "GetProductsByCategory" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"

    Examples:
      | tcNo  | description                     | pathParams             | queryParams | headers | statusCode | statusText |
      | PC001 | Get electronics - page 1        | categoryId=electronics | page=1      | none    | 200        | OK         |
      | PC002 | Get clothing - page 1           | categoryId=clothing    | page=1      | none    | 200        | OK         |
      | PC003 | Get books - page 1              | categoryId=books       | page=1      | none    | 200        | OK         |
      | PC004 | Get electronics - page 2        | categoryId=electronics | page=2      | none    | 200        | OK         |
      | PC005 | Get home-garden category        | categoryId=home-garden | page=1      | none    | 200        | OK         |
