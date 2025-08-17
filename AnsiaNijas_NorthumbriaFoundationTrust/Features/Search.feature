@search
Feature: Search Functionality on Northumbria NHS Website
  As a Northumbria foundation user
  I want to be able to perform a search  by button or Enter for "Quality and safety"
  So that I can view relevant information


  @button
  Scenario Outline: Submit search via Search button
    Given I navigate to the Northumbria NHS homepage
    When I enter "<term>" in the search box
    And I perform the search by clicking the search button
    Then I should see search results related to "<term>"
    And I can click the "Quality and safety" link from the results
    And I navigate to the "Continually improving services" page
    Then I should see relevant information about the section

    Examples:
      | term               |
      | Quality and safety |

  @enter
  Scenario Outline: Submit search via Enter key
    Given I navigate to the Northumbria NHS homepage
    When I enter "<term>" in the search box
    And I perform the search by enter
    Then I should see search results related to "<term>"
    And I can click the "Quality and safety" link from the results
    And I navigate to the "Continually improving services" page
    Then I should see relevant information about the section

    Examples:
      | term               |
      | Quality and safety |
