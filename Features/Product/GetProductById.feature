Feature: GetProductById - Retrieve specific product by ID

  Background:
    Given I have the "ProductAPI" service initialized

  Scenario Outline: GetProductById endpoint tests
    When I create a GET request to "GetProductById" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"
    And I verify that the field "<fieldName>" in the response is "<fieldValue>"
    And I verify the "<fieldType>" type in the response to be a "<expectedType>"
    And I verify the "<fieldExists>" in the response "exists"
    And the response returned should match the schema "<schemaFile>"

    Examples:
      | tcNo  | description                | pathParams | queryParams | headers | statusCode | statusText | fieldName      | fieldValue | fieldType      | expectedType | fieldExists       | schemaFile                    |
      | PG001 | Get product 101            | id=101     | none        | none    | 200        | OK         | data.id        | 101        | data.id        | integer      | data              | get-product-by-id-schema.json |
      | PG002 | Get product 102            | id=102     | none        | none    | 200        | OK         | data.id        | 102        | data.id        | integer      | data.name         | get-product-by-id-schema.json |
      | PG003 | Get product 103 - price    | id=103     | none        | none    | 200        | OK         | data.price     | NotEmpty   | data.price     | number       | data.price        | get-product-by-id-schema.json |
      | PG004 | Get product 104 - stock    | id=104     | none        | none    | 200        | OK         | data.stock     | NotEmpty   | data.stock     | integer      | data.stock        | get-product-by-id-schema.json |
      | PG005 | Get product 999 - not found| id=999     | none        | none    | 404        | Not Found  | n/a            | n/a        | n/a            | n/a          | n/a               | none                          |
