Feature: SearchProducts - Search products with query, sort, and filters

  Background:
    Given I have the "ProductAPI" service initialized

  Scenario Outline: SearchProducts endpoint tests
    When I create a GET request to "SearchProducts" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"

    Examples:
      | tcNo  | description                          | pathParams | queryParams                    | headers | statusCode | statusText |
      | PS001 | Search laptop - sort by price asc    | none       | q=laptop&sort=price&order=asc  | none    | 200        | OK         |
      | PS002 | Search phone - sort by name          | none       | q=phone&sort=name              | none    | 200        | OK         |
      | PS003 | Search tablet - sort by price desc   | none       | q=tablet&sort=price&order=desc | none    | 200        | OK         |
      | PS004 | Search headphones                    | none       | q=headphones                   | none    | 200        | OK         |
      | PS005 | Search laptop - sort by rating       | none       | q=laptop&sort=rating&order=desc| none    | 200        | OK         |
