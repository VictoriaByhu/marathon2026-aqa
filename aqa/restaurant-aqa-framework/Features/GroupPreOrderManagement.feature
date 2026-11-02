@extra
Feature: Managing a group pre-order as the host

  Beyond seeing, submitting and removing, the host looks after the group:
  corrects a guest's order, reminds guests who are still choosing and follows
  what happens in the group. The reservation card leads straight to it.

  Background:
    Given the restaurant is ready to take group pre-orders
    And a customer has booked a table for a group of 6

  Rule: The owner can correct a guest's order

    @api @smoke
    Scenario: The owner changes what a guest has chosen
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      When the owner changes "Anna"'s order to:
        | dish | quantity |
        | A    | 3        |
        | B    | 1        |
      Then the owner sees every participant with their dishes, quantities and subtotal
      And the overall total is the sum of the participants' subtotals
      And "Anna" sees the same order in the cart

    @api
    Scenario: A corrected cart that was already submitted goes to the restaurant as corrected
      Given the owner has pre-ordered for themselves:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has joined the group and chosen:
        | dish | quantity |
        | B    | 2        |
      And "Anna" has submitted the cart
      When the owner changes "Anna"'s order to:
        | dish | quantity |
        | B    | 1        |
      And the owner submits the group pre-order
      Then the pre-order is sent to the restaurant
      And the restaurant receives:
        | dish | quantity |
        | A    | 1        |
        | B    | 1        |

    @api @negative
    Scenario: A submitted cart cannot be emptied
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has submitted the cart
      When the owner tries to empty "Anna"'s cart
      Then the owner is told "A submitted cart cannot be left empty. Remove the guest instead."

    @api @negative
    Scenario: Only dishes of this restaurant can be added
      Given "Anna" has joined the group
      When the owner tries to add a dish from another restaurant to "Anna"'s order
      Then the owner is told that the dish is not on the restaurant's menu

    @api
    Scenario: The owner sees the dishes that can be added
      When the owner opens the menu of the group's restaurant
      Then the menu offers the dishes the guests can order

    @api @negative @security
    Scenario Outline: Someone other than the owner cannot change a guest's order
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      When <someone> tries to change "Anna"'s order
      Then the request is refused because <reason>
      And "Anna" still has the same order in the cart

      Examples:
        | someone              | reason                 |
        | another customer     | it is not their group  |
        | the waiter           | it is not their group  |
        | a guest of the group | they are not signed in |
        | an anonymous visitor | they are not signed in |

    @ui @smoke
    Scenario: The owner corrects a guest's order on the review page
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      When the owner opens the group pre-order review page
      And the owner changes "Anna"'s order on the page to:
        | dish | quantity |
        | A    | 2        |
        | B    | 1        |
      Then the review page shows "Anna" with:
        | dish | quantity |
        | A    | 2        |
        | B    | 1        |
      And "Anna" sees the same order in the cart

  Rule: The owner can remind guests who are still choosing

    @api @smoke
    Scenario: The owner reminds a guest
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      When the owner reminds "Anna"
      Then "Anna" is reminded to submit the cart

    @api
    Scenario: The owner reminds everyone who is still choosing
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has submitted the cart
      And "Bohdan" has joined the group
      And "Olena" has joined the group and chosen:
        | dish | quantity |
        | B    | 1        |
      When the owner reminds everyone who is still choosing
      Then the reminder goes to:
        | guest  |
        | Bohdan |
        | Olena  |
      And "Anna" is not reminded

    @api @negative
    Scenario: A guest who has already submitted is not reminded
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has submitted the cart
      When the owner reminds "Anna"
      Then the owner is told "This guest has already submitted the cart."

    @api @negative @security
    Scenario Outline: Someone other than the owner cannot send reminders
      Given "Anna" has joined the group
      When <someone> tries to remind "Anna"
      Then the request is refused because <reason>
      And "Anna" is not reminded

      Examples:
        | someone              | reason                 |
        | another customer     | it is not their group  |
        | the waiter           | it is not their group  |
        | a guest of the group | they are not signed in |
        | an anonymous visitor | they are not signed in |

    @ui
    Scenario: The guest sees the host's reminder
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      When the owner opens the group pre-order review page
      And the owner sends "Anna" a reminder on the page
      Then the review page shows that "Anna" was reminded
      When "Anna" opens the guest pre-order page
      Then "Anna" sees the host's reminder

  Rule: The owner sees what happened in the group

    @api @smoke
    Scenario: The activity tells what the guests and the owner did, newest first
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 2        |
      And "Anna" has submitted the cart
      And "Bohdan" has joined the group
      And the owner has removed "Bohdan" from the group
      When the owner looks at the group's activity
      Then the activity reads, newest first:
        | event              | guest  |
        | removed by owner   | Bohdan |
        | joined             | Bohdan |
        | submitted the cart | Anna   |
        | added a dish       | Anna   |
        | joined             | Anna   |

    @api @negative @security
    Scenario Outline: Someone other than the owner cannot see the activity
      Given "Anna" has joined the group
      When <someone> tries to look at the group's activity
      Then the request is refused because <reason>

      Examples:
        | someone              | reason                 |
        | another customer     | it is not their group  |
        | the waiter           | it is not their group  |
        | a guest of the group | they are not signed in |
        | an anonymous visitor | they are not signed in |

    @ui
    Scenario: The review page shows the recent activity
      Given "Anna" has joined the group and chosen:
        | dish | quantity |
        | A    | 1        |
      And "Anna" has submitted the cart
      When the owner opens the group pre-order review page
      And the owner opens the recent activity
      Then the recent activity mentions "Anna submitted the cart"

  Rule: The reservation leads to its group pre-order

    @ui @smoke
    Scenario: The owner finds the group pre-order from the reservation card
      When the owner chooses the group pre-order on the reservation card
      Then the group pre-order review page of the reservation is shown
