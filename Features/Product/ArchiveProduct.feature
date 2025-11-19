Feature: ArchiveProduct - Soft delete product with reason

  Background:
    Given I have the "ProductAPI" service initialized

  Scenario Outline: ArchiveProduct endpoint tests
    When I create a DELETE request to "ArchiveProduct" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"

    Examples:
      | tcNo  | description                      | pathParams | queryParams         | headers | statusCode | statusText |
      | PV001 | Archive product - discontinued   | id=102     | reason=Discontinued | none    | 200        | OK         |
      | PV002 | Archive product - out of stock   | id=103     | reason=OutOfStock   | none    | 200        | OK         |
      | PV003 | Archive product - quality issue  | id=104     | reason=QualityIssue | none    | 200        | OK         |
      | PV004 | Archive product - obsolete       | id=105     | reason=Obsolete     | none    | 200        | OK         |
