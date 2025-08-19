@search
Feature: Search Functionality on Northumbria NHS Website
  As a Northumbria foundation user
  I want to be able to perform a search  by button or Enter for "Quality and safety"
  So that I can view relevant information


@happypath
  Scenario Outline: Perform a search by clicking the search button as well as pressing the enter key on the keyboard
    Given I navigate to the Northumbria NHS homepage
    When I enter "<term>" in the search box
    And I perform the search using the "<trigger>" action
    Then I should see search results related to "<term>"
    And I can click the "<term>" link from the results
    And I navigate to the "Continually improving services" page by clicking on the box 
    Then I should see relevant information about "Continually improving services"
    
    Examples:
      | term               |Trigger|
      | Quality and safety |search |
      | Quality and safety |enter  |
