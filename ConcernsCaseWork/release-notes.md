## 31.0.0

## 30.0.0

* Ensure no tables have empty headers and all have a caption
* Content improvements for regions group journey
* Optimisic searching for a city technical college returns a 204 instead of 404
* Added indexes to trust UK PRN and created by on the case table
* Added selectable actions to regions group journey
* Changed column layout of case tables
* Reduced the number of calls to redis

## 29.0.1
* fixed caching issue with conversion from non concerns to concerns, incorrect trust data was displayed in the summary

## 29.0.0
* Content changes to TFF, so the acronym appears first
* New page for recording the manager of a case
* Territory page has been brought forward to after the managed by page
* Content improvements in the case creation journey
* Fixed the swagger API docs
* Added pagination to the team casework page
* Fixed issue where text areas jump when typing a large amount of text. Text areas can be manually expanded by the user as needed
* Department For Education styling has been applied
* Concern type nesting has been removed and names have changed

## 28.0.0
* Add Emergency funding option to Framework Categories for Repayable and Non Repayable Decision Types
* Soft Delete SRMA via Api
* Soft Delete Financial Plan via Api
* Soft Delete Case via Api
* Soft Delete Nti Under Consideration via Api

## 27.1.0
* Change the RMF guidance link

## 27.0.0
* Display sub questions for three deicsion types to capture information on related funding
* Your/Team casework Last Updated field reflects the date of most recent change when creating or updating a concern and any case action.
* Enhanced Pagination with the use of Ellipsis
* Amends to risk rating hint text
* Amends to Find Trust and Create Case text

## 26.0.0
* Ability to add Cases to CTCs
* Remove redundant code replace by components
* Enabling work on Api for adding Sub questions to Decision work
* Fix incorrect character count display in UI
* Ability to Soft Delete TFF for a Case via Api
* Amend subnav on trust page to better reflect the design

## 25.0.0
* Add pagination to get active cases by owner and get closed cases by owner APIs
* Your active casework redesign - Add pagination
* Trust Overview Redesign - Add Open/Closed sub-navs
* Trust Overview Design - Add pagination to sub-navs

## 24.0.0
* Increase SRMA notes field limit
* Iterate hint text on Create Case page based on user feedback
* Amend Create case button to adopt combined Concerns and Non Concerns Journey
* Fixed radio button selection changing values in another radio group

## 23.0.0

* Ability to change case owner using keyboard only input
* Ability to select Trust in Trust Search using keyboard only input
* Fix typo on Error Message Page
* Ensure consistent use of term Case Owner
* Show Error Summary and Error message next to each answer for Reassign Case Owner, Close Case and Find a Trust Error Pages
* Ensure Error Message order is consistent with field on case creation page
* Fix issue with Decision ID not being visible in Closed Notice to Improve Information
* Ability to return paged results to Get Case By Trust Endpoint
* Add Feature toggle to switch to Updated Trust Search Endpoint

## 22.0.0

* Added ability to Create a Concerns case from a Proactive Engagement Case (Non Concerns case)
* Added soft delete functionality to Concerns, Decision, Notice to Improve and Notice to Improve Warning Letter
* Provactive Engagement Case layout changes
* Error message improvements for Trust Financial Forecast, Decisions & SRMA error messages
* New KPI to track when case owner is changed
* Added Fake Trust to enable Production smoke testing

## 21.0.0

* Amended Case Create flow to allow for non concerns
* Added logging of page views to App Insights
* Improved error messages by adding create context to guide the users across a number of pages
* Added hyperlinks to error message summaries 
* Added feature flag for non concerns page
* Added ability to show SRMA dates within summary table
---
## 20.0.0

* Improved the hint text for reassign case owner to make instructions clearer
* Improved validation on the NTI close page to validate the date closed and retain the information if an incorrect input is received
* Added a link to the case archive on the closed case page, for cases that have been migrated from DaRT

---

## 19.0.0

* Added Companies House Number to concerns cases. Each new concerns case will store the Companies House Number of the associated Trust (existing cases will be updated in retrospect).
* Updated wording on the page that is displayed when the user does not have access to the system.

---

## 18.0.1
* CSS styling fix on the Re-Assign page.
* Changed 'Edit' link to 'Change' on the Case/Management/Index page

---

## 18.0.0
* Re-Assign Cases to other users
* Trust Financial Forecast Enhancements
* Small UI and usability enhancements
* Added integration points for Microsoft Graph API

---

## 17.0.1 - Hotfixes
* Handle validation with new lines in javascript.
* Optimised calls to the academies api for trust search to address latency spike.
* 121740: Cypress test JS validation issue by.
* 121903: Add close tag to each closed case and action by.

---

# 17.0.0 - Initial release of Record Concerns
Initial Release of the Record-Concerns-and-Decisions system into production.
For full details see project documentation.
