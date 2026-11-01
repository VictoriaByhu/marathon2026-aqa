@level-1
Feature: Seeing who has joined a group pre-order

  The owner of a group reservation invites guests. Every guest who follows the
  invitation gets a cart of their own. The owner sees everyone who has joined,
  what each of them has chosen, how much each cart costs and what the whole
  group adds up to.

  Background:
    Given the restaurant is ready to take group pre-orders
    And a customer has booked a table for a group of 6

  Rule: The owner sees every participant with their dishes and totals

    @api @smoke
    Scenario: The owner sees what each guest has chosen
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 2        |
        | B    | 1        |
      And "Anna" has submitted the cart
      And "Bohdan" has joined the group and chosen:
        | dish | quantity |
        | B    | 3        |
      When the owner looks at the group pre-order
      Then the owner sees every participant with their dishes, quantities and subtotal
      And the overall total is the sum of the participants' subtotals

    @api
    Scenario: The overall total grows when a guest adds a dish
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      And the owner has looked at the group pre-order
      When "Anna" adds 2 of dish "B"
      And the owner looks at the group pre-order
      Then the overall total has grown by the price of 2 of dish "B"
      And the overall total is the sum of the participants' subtotals

    @api
    Scenario: The overall total shrinks when a guest takes a dish out
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 3        |
        | B    | 1        |
      And the owner has looked at the group pre-order
      When "Anna" takes 1 of dish "A" out of the cart
      And the owner looks at the group pre-order
      Then the overall total has dropped by the price of 1 of dish "A"
      And the owner sees every participant with their dishes, quantities and subtotal

    @ui @smoke
    Scenario: The owner sees the guests' carts on the group pre-order page
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 2        |
      And "Anna" has submitted the cart
      And "Bohdan" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
        | B    | 2        |
      When the owner opens the group pre-order page
      Then the owner sees every participant with their dishes, quantities and subtotal
      And the grand total covers only the submitted carts

  Rule: Participants who have not finished are still listed

    @api
    Scenario: A guest who is still choosing is marked as not submitted
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      When the owner looks at the group pre-order
      Then "Anna" is listed as still choosing

    @api
    Scenario: A guest who has not chosen anything is listed with an empty cart
      Given "Bohdan" has joined the group
      When the owner looks at the group pre-order
      Then "Bohdan" is listed without any dishes

    @ui
    Scenario: The page marks guests who are still choosing or have no dishes
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | B    | 1        |
      And "Bohdan" has joined the group
      When the owner opens the group pre-order page
      Then "Anna" is listed as still choosing
      And "Bohdan" is listed without any dishes

  Rule: A group nobody has joined is empty

    @api
    Scenario: The owner looks at a group nobody has joined
      When the owner looks at the group pre-order
      Then the group has no participants and nothing to pay

    @ui
    Scenario: The page tells the owner that nobody has added dishes yet
      When the owner opens the group pre-order page
      Then the owner is told that the participants have not added any dishes yet

  Rule: Only the owner of the group can see its participants

    @api @negative @security
    Scenario Outline: Someone other than the owner cannot see the participants
      Given "Anna" has joined the group
      When <someone> asks to see the participants of the group
      Then the request is refused because <reason>

      Examples:
        | someone              | reason                 |
        | another customer     | it is not their group  |
        | the waiter           | it is not their group  |
        | a guest of the group | they are not signed in |
        | an anonymous visitor | they are not signed in |

  Rule: A group that does not exist cannot be viewed

    @api @negative
    Scenario: The owner asks for a group that does not exist
      When the owner asks to see the participants of a group that does not exist
      Then the owner is told that the group could not be found
