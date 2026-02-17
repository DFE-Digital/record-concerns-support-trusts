# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). To see an example from a mature product in the program [see the Complete products changelog that follows the same methodology](https://github.com/DFE-Digital/complete-conversions-transfers-changes/blob/main/CHANGELOG.md).

--- 
  
## [Unreleased](https://github.com/DFE-Digital/record-concerns-support-trusts/compare/production-2026-02-16.1060..HEAD)

- 265506 - Recast RMF Categories Selectable on Concerns added to existing cases
---

## [63.0.0][63.0.0] - 2026-02-17

### Added
- 257479 -  Add to case - Text field label not announcing via hint text
- 260375 - Team Leader Selection - Creation Journey
- 262054 - Inclusion of RAG Status Rational - Creation Journey

## Fixed 
- 264154 -Recast- Team leader - pull from 2 entra groups

---
## [62.2.0][62.2.0] - 2026-02-12

### Fixed
- 263587 - Remove null value records or email from Ad group search result for Team Leader functionality 

---

## [62.1.0][62.1.0] - 2026-02-10

### Changed 
- 257482 -Accessibility Add to case - Required field not indicated

---

## [62.0.0][62.0.0] - 2026-02-06

### Added
- 257480 - added fieldset to correctly group radio buttons to legends on add to case > decision page
- 258005 - added ability to change the team leader selection

### Changed
- 142564 - Update SFSO concern types to match the RMF
- 257483 - accessibility error summary update, add role alert for screen reader
- 257481 - accessibility update, hint text had incorrect association

### Fixed
- 258984 - fix trust summary pupil capacity percentage calculation

---

## [61.0.0][61.0.0] - 2026-01-20

### Changed
- 255793 - update accessibility statement link in footer

---

## [60.0.0][60.0.0] - 2026-01-14

### Changed
- 253089 - Replace all references to text 'ESFA' to 'SFSO'

---

## [59.0.0][59.0.0] - 2025-12-03

### Removed
- 251429 - Remove EAG radio button option in repayable finance support section

---

## [58.0.0][58.0.0] - 2025-12-01

### Added
- 245729 - New DB fields and changes to front end labels
- 245734 - Decision supporting notes mandatory
- 245737 - add Exceptional Annual Grant (EAG) decision type
- 247611 - Reserves Oversight & Assurance changes to support lower priority work
- 245732 - create new drawdown option 'final drawdown'
- 245735 - Remove countersigning option & Add Secretary of State

### Fixed
- Fix broken ef migrations

---

## [57.0.0][57.0.0] - 2025-11-06

### Added
- docker support for local development
- 240534 - new header, new footer, service navigation, cookie banner
- 244480 - Add FinancialRecoveryOfFraudOrIrregularity to ConcernsDecisionTypeId

---

## [56.0.0][56.0.0] - 2025-10-10

### Added
- 237612 - Design - Inclusion of new Decision Type - Financial Recovery

### Fixed
- 202716 - accessibility issue in header navigation
- Change secrets id

### Security
- Package Validator policy update

---

## [55.0.0][55.0.0] - 2025-08-26

---

## [54.0.0][54.0.0] - 2025-08-19

### Added
- 225653 - Enabled retry logic on failure to connect to SQL database

### Security
- renovate dependency updates

---

## [53.0.0][53.0.0] - 2025-07-31

### Changed
- 226384 - Changed Risk management URL

### Security
- renovate dependency updates

---

## [52.0.0][52.0.0] - 2025-02-21

### Security

- renovate dependency updates

---

## [51.0.0][51.0.0] - 2025-02-21

### Security
- renovate dependency updates

---

## [50.0.0][50.0.0] - 2025-02-07

### Removed
- Remove Azure Front Door CDN

---

## [49.0.0][49.0.0] - 2025-02-07

### Added
- 195187 - Added Package Validation gate to the workflows

---

## [48.0.0][48.0.0] - 2025-02-05

### Fixed
- Build concurrency

---

## [47.0.0][47.0.0] - 2025-02-05

### Fixed
- 198645 - Added TTE action validation on either closing or deleting case

---

## [46.0.0][46.0.0] - 2025-01-23

### Changed
- updated renovate.json to inherit config from the shared renovate repo

---

## [45.0.0][45.0.0] - 2025-01-23

### Changed
- updated renovate.json to inherit config from the shared renovate repo

---

## [44.0.0][44.0.0] - 2025-01-16

### Changed
- Updated renovate config to extend the shared config

---

## [43.0.0][43.0.0] - 2025-01-07

### Fixed
- 193980 - Display closed concerns with correct status

---

## [42.0.0][42.0.0] - 2025-01-06

### Security
- Update Terraform github.com/DFE-Digital/terraform-azurerm-container-apps-hosting

---

## [41.0.0][41.0.0] - 2025-01-06

### Security
- Update DFE-Digital/deploy-azure-container-apps-action action

---

NOTE: potentiall missing releases here

--- 
  
## 40.0.0
* added Targeted Trust Engagement and hid Trust Financial Forecase

--- 
  
## 39.0.0

* added delete button to the case, concerns and actions

--- 
  
## 38.2.0

* updated the footer with how to guidance and remove line break

--- 
  
## 38.1.0

* updated the footer with new UCD changes

--- 
  
## 38.0.1

* bug fix to show decision type/title on trust cases overview

--- 
  
## 38.0.0

* updated the description of the SFSO territories and removed North and UTC - UTC and National Operations as options

--- 
  
## 37.0.0

* added whistleblowing as a new option for means of referral
* updated the styling so that all the content is in line with the website header

--- 
  
## 36.0.0

* added a notification banner to inform users of system changes, only displays when they are scheduled

--- 
  
## 35.0.0

* moved over to the shared deployment infrastructure
* images are now in the github container registry

--- 
  
## 34.0.0

* added maintenance page for notifying users when system updates are happening


--- 
  
## 33.0.0

* added close concern to the case management page
* removed close concern from the edit concern page
* fixed issue where text on case narratives could not be completely removed

--- 
  
## 32.0.0

* added a navigation header to make it easier to select 'Your cases', 'Closed cases', 'Other cases' and 'Find trust'
* re-designed the select colleages page
* updated the accessibility statement
* added improved validation to the create and update case APIs
* renamed concern type 'governance' to 'governance capability'
* added the RMF link for regions group 

--- 
  
## 31.0.0

* Change Add concern to Add another concern in the case creation wizard
* Add trust nane column to the other cases table
* Increase the default size of text areas
* Reduced the number of calls to redis

--- 
  
## 30.0.0

* Ensure no tables have empty headers and all have a caption
* Content improvements for regions group journey
* Optimisic searching for a city technical college returns a 204 instead of 404
* Added indexes to trust UK PRN and created by on the case table
* Added selectable actions to regions group journey
* Changed column layout of case tables
* Reduced the number of calls to redis

--- 
  
## 29.0.1
* fixed caching issue with conversion from non concerns to concerns, incorrect trust data was displayed in the summary

--- 
  
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

--- 
  
## 28.0.0
* Add Emergency funding option to Framework Categories for Repayable and Non Repayable Decision Types
* Soft Delete SRMA via Api
* Soft Delete Financial Plan via Api
* Soft Delete Case via Api
* Soft Delete Nti Under Consideration via Api

--- 
  
## 27.1.0
* Change the RMF guidance link

--- 
  
## 27.0.0
* Display sub questions for three deicsion types to capture information on related funding
* Your/Team casework Last Updated field reflects the date of most recent change when creating or updating a concern and any case action.
* Enhanced Pagination with the use of Ellipsis
* Amends to risk rating hint text
* Amends to Find Trust and Create Case text

--- 
  
## 26.0.0
* Ability to add Cases to CTCs
* Remove redundant code replace by components
* Enabling work on Api for adding Sub questions to Decision work
* Fix incorrect character count display in UI
* Ability to Soft Delete TFF for a Case via Api
* Amend subnav on trust page to better reflect the design

--- 
  
## 25.0.0
* Add pagination to get active cases by owner and get closed cases by owner APIs
* Your active casework redesign - Add pagination
* Trust Overview Redesign - Add Open/Closed sub-navs
* Trust Overview Design - Add pagination to sub-navs

--- 
  
## 24.0.0
* Increase SRMA notes field limit
* Iterate hint text on Create Case page based on user feedback
* Amend Create case button to adopt combined Concerns and Non Concerns Journey
* Fixed radio button selection changing values in another radio group

--- 
  
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

--- 
  
## 22.0.0

* Added ability to Create a Concerns case from a Proactive Engagement Case (Non Concerns case)
* Added soft delete functionality to Concerns, Decision, Notice to Improve and Notice to Improve Warning Letter
* Provactive Engagement Case layout changes
* Error message improvements for Trust Financial Forecast, Decisions & SRMA error messages
* New KPI to track when case owner is changed
* Added Fake Trust to enable Production smoke testing

--- 
  
## 21.0.0

* Amended Case Create flow to allow for non concerns
* Added logging of page views to App Insights
* Improved error messages by adding create context to guide the users across a number of pages
* Added hyperlinks to error message summaries 
* Added feature flag for non concerns page
* Added ability to show SRMA dates within summary table
---
--- 
  
## 20.0.0

* Improved the hint text for reassign case owner to make instructions clearer
* Improved validation on the NTI close page to validate the date closed and retain the information if an incorrect input is received
* Added a link to the case archive on the closed case page, for cases that have been migrated from DaRT

---

--- 
  
## 19.0.0

* Added Companies House Number to concerns cases. Each new concerns case will store the Companies House Number of the associated Trust (existing cases will be updated in retrospect).
* Updated wording on the page that is displayed when the user does not have access to the system.

---

--- 
  
## 18.0.1
* CSS styling fix on the Re-Assign page.
* Changed 'Edit' link to 'Change' on the Case/Management/Index page

---

--- 
  
## 18.0.0
* Re-Assign Cases to other users
* Trust Financial Forecast Enhancements
* Small UI and usability enhancements
* Added integration points for Microsoft Graph API

---

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

[63.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2026-02-16.1060
[62.2.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2026-02-11.1039
[62.1.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2026-02-10.1031
[62.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2026-02-06.1024
[61.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2026-01-20.989
[60.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2026-01-14.984
[59.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-12-03.981
[58.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-12-01.977
[57.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-11-06.924
[56.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-10-10.887
[55.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-08-26.874
[54.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-08-19.870
[53.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-07-31.857
[52.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-02-21.784
[51.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-02-21.783
[50.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-02-07.774
[49.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-02-07.772
[48.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-02-05.771
[47.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-02-05.765
[46.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-01-23.761
[45.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-01-23.760
[44.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-01-16.759
[43.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-01-07.757
[42.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-01-06.754
[41.0.0]: https://github.com/DFE-Digital/record-concerns-support-trusts/releases/tag/production-2025-01-06.751
