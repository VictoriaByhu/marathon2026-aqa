@level-2
Feature: Submitting the consolidated group pre-order

  Once the owner is happy with what the guests have chosen, the owner sends one
  consolidated pre-order to the restaurant. Only carts the guests have submitted
  are included, the same dish ordered by several people becomes one line with
  the total quantity, and the dishes join the owner's own pre-order.

  Background:
    Given the restaurant is ready to take group pre-orders
    And a customer has booked a table for a group of 6

  Rule: Only submitted carts are sent, with the same dishes merged

    @api @smoke
    Scenario: The owner submits the consolidated pre-order
      Given the owner has pre-ordered for themselves:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 2        |
        | B    | 1        |
      And "Anna" has submitted the cart
      And "Bohdan" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      And "Bohdan" has submitted the cart
      And "Olena" has joined the group and chosen:
        | dish | quantity |
        | B    | 4        |
      When the owner submits the group pre-order
      Then the pre-order is sent to the restaurant
      And the restaurant receives:
        | dish | quantity |
        | A    | 4        |
        | B    | 1        |
      And the group is marked as submitted

    @api
    Scenario: The waiter sees the order the owner submitted
      Given the owner has pre-ordered for themselves:
        | dish | quantity |
        | B    | 1        |
      And "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 2        |
      And "Anna" has submitted the cart
      When the owner submits the group pre-order
      Then the waiter sees the order with:
        | dish | quantity |
        | A    | 2        |
        | B    | 1        |

    @api
    Scenario: The owner has a place of their own in the group
      Given "Anna" has joined the group
      When the owner takes their own place in the group
      And the owner looks at the group pre-order
      Then the owner is listed first as the host of the group
      And the owner sees every participant with their dishes, quantities and subtotal

    @ui @smoke
    Scenario: The owner submits the pre-order from the group pre-order page
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
      When the owner opens the group pre-order page
      And the owner submits the pre-order on the page
      Then the page shows that the pre-order was sent to the restaurant
      And the restaurant receives:
        | dish | quantity |
        | A    | 3        |

  Rule: There has to be something to send

    @api @negative
    Scenario: Nobody has submitted a cart yet
      Given the owner has pre-ordered for themselves:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has joined the group and chosen:
        | dish | quantity |
        | B    | 1        |
      When the owner submits the group pre-order
      Then the owner is told "There are no submitted guest sessions in this group."

    @api @negative
    Scenario: A guest cannot submit an empty cart, so a submitted cart always has dishes
      Given "Anna" has joined the group
      When "Anna" tries to submit the cart
      Then the guest is told "The cart is empty"

    @api @negative
    Scenario: The owner has no pre-order of their own to add the dishes to
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has submitted the cart
      When the owner submits the group pre-order
      Then the owner is told that the order of the reservation could not be found

    @api @negative
    Scenario: The group does not exist
      When the owner submits the pre-order of a group that does not exist
      Then the owner is told that the group could not be found

    @api @negative
    Scenario: The reservation does not exist
      When the owner submits the group pre-order for a reservation that does not exist
      Then the owner is told that the reservation could not be found

    @ui @negative
    Scenario: The page does not let the owner submit before any guest has
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      When the owner opens the group pre-order page
      Then the owner cannot submit the pre-order on the page

  Rule: A group pre-order is submitted only once

    @api @negative
    Scenario: The owner submits a group that has already been submitted
      Given the owner has pre-ordered for themselves:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has joined the group and chosen:
        | dish | quantity |
        | B    | 2        |
      And "Anna" has submitted the cart
      And the owner has submitted the group pre-order
      When the owner submits the group pre-order
      Then the owner is told "Group reservation was submitted."
      And the restaurant receives:
        | dish | quantity |
        | A    | 1        |
        | B    | 2        |

  Rule: Only the owner can submit the group pre-order

    @api @negative @security
    Scenario Outline: Someone other than the owner cannot submit
      Given the owner has pre-ordered for themselves:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has joined the group and chosen:
        | dish | quantity |
        | B    | 1        |
      And "Anna" has submitted the cart
      When <someone> tries to submit the group pre-order
      Then the request is refused because <reason>
      And the group is still collecting pre-orders

      Examples:
        | someone              | reason                 |
        | another customer     | it is not their group  |
        | the waiter           | it is not their group  |
        | a guest of the group | they are not signed in |
        | an anonymous visitor | they are not signed in |
