@level-3
Feature: Removing a participant from a group pre-order

  The owner can take a participant out of the group. The participant's cart goes
  with them, and the owner sees the updated list and total straight away.

  Background:
    Given the restaurant is ready to take group pre-orders
    And a customer has booked a table for a group of 6

  Rule: Removing a participant updates the group

    @api @smoke
    Scenario: The owner removes one of several guests
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 2        |
      And "Bohdan" has joined the group and chosen:
        | dish | quantity |
        | B    | 3        |
      And "Olena" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      When the owner removes "Bohdan" from the group
      Then "Bohdan" is no longer in the group
      And the owner sees every participant with their dishes, quantities and subtotal
      And the overall total is the sum of the participants' subtotals

    @api
    Scenario: The owner removes a guest who has already submitted the cart
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has submitted the cart
      And "Bohdan" has joined the group and chosen:
        | dish | quantity |
        | B    | 1        |
      When the owner removes "Anna" from the group
      Then "Anna" is no longer in the group
      And the owner sees every participant with their dishes, quantities and subtotal

    @api
    Scenario: The owner removes the last guest
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | B    | 2        |
      When the owner removes "Anna" from the group
      Then the group has no participants and nothing to pay

    @api @negative
    Scenario: The owner cannot remove their own place in the group
      Given the owner has taken their own place in the group
      When the owner tries to remove their own place from the group
      Then the owner is told "Owner session cannot be removed from the group."

    @ui @smoke
    Scenario: The owner removes a guest on the group pre-order page
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has submitted the cart
      And "Bohdan" has joined the group and chosen:
        | dish | quantity |
        | B    | 2        |
      And "Bohdan" has submitted the cart
      When the owner opens the group pre-order page
      And the owner removes "Bohdan" on the page
      Then the owner sees every participant with their dishes, quantities and subtotal
      And the grand total covers only the submitted carts

  Rule: Only a participant of this group can be removed

    @api @negative
    Scenario: The participant belongs to another group
      Given the customer has made a second group booking for 4 guests
      And "Olena" has joined the second group
      And "Anna" has joined the group
      When the owner tries to remove "Olena" from the group
      Then the owner is told "Guest session does not belong to this group reservation."
      And "Olena" is still in the second group

    @api @negative
    Scenario: The participant does not exist
      Given "Anna" has joined the group
      When the owner tries to remove someone who never joined the group
      Then the owner is told that the participant could not be found

  Rule: Only the owner can remove participants

    @api @negative @security
    Scenario Outline: Someone other than the owner cannot remove a participant
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      When <someone> tries to remove "Anna" from the group
      Then the request is refused because <reason>
      And "Anna" is still in the group

      Examples:
        | someone              | reason                 |
        | another customer     | it is not their group  |
        | the waiter           | it is not their group  |
        | a guest of the group | they are not signed in |
        | an anonymous visitor | they are not signed in |

  Rule: The whole journey from the guests' carts to the waiter

    @ui @e2e
    Scenario: Guests order, the owner removes one and submits, the waiter gets the rest
      Given the owner has pre-ordered for themselves:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 2        |
      And "Anna" has submitted the cart
      And "Bohdan" has joined the group and chosen:
        | dish | quantity |
        | B    | 1        |
      And "Bohdan" has submitted the cart
      And "Olena" has joined the group and chosen:
        | dish | quantity |
        | B    | 2        |
      And "Olena" has submitted the cart
      When the owner opens the group pre-order page
      And the owner removes "Bohdan" on the page
      And the owner submits the pre-order on the page
      Then the page shows that the pre-order was sent to the restaurant
      When the waiter opens the pre-order of the reservation
      Then the waiter sees the order with:
        | dish | quantity |
        | A    | 3        |
        | B    | 2        |
