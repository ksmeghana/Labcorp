Feature: LabCorp Job Search and Apply
  As a job seeker
  I want to search and apply for jobs at LabCorp
  So that I can find a suitable position for my career

Background:
  Given I open the browser and navigate to "https://www.labcorp.com"
  When I click the "Careers" link

Scenario: Search for Senior Full Stack Engineer Position
  When I search for the position "Senior Full Stack Engineer"
  And I select the job from the results
  Then I should see the job details with:
    | Job Title    | Senior Full Stack Engineer                       |
    | Job Location | Durham, North Carolina, United States of America |
    | Job Id       | 2517089                                          |
    | Job Category | Information Technology                           |
    | Job Type     | Full-Time                                        |
    | Job Industry | Professional Services                            |

Scenario: Verify Job Description Details
  When I search for the position "Senior Full Stack Engineer"
  And I select the job from the results
  Then the job description should contain:
    | Duties & Responsibilities | Develop through modern Agile development methodologies intuitive, easy-to-use software in collaboration with the development team, project managers, business analysts, UX designers, quality assurance and users across the organization. |
    | Minimum Requirement      | Bachelor's degree in computer science or equivalent technical work experience.                                                                                                                                                         |
    | Preferred Requirement    | 2+ years work experience and expert knowledge of primary AWS services (Lambda, ECS, IAM, VPC, EC2, ELB, RDS, Route53, S3, API gateway, SQS, DynamoDB).                                                                                 |
    | Application Window      | Application Window closes 8/10/2025                                                                                                                                                                                                    |
    | Benefits               | Employees regularly scheduled to work 20 or more hours per week are eligible for comprehensive benefits including: Medical, Dental, Vision, Life, STD/LTD, 401(k), Paid Time Off (PTO) or Flexible Time Off (FTO), Tuition Reimbursement and Employee Stock Purchase Plan. |
    | Equal Opportunity      | Labcorp is proud to be an Equal Opportunity Employer: Labcorp strives for inclusion and belonging in the workforce and does not tolerate harassment or discrimination of any kind.                                                     |

Scenario: Apply for Position and Return to Search
  When I search for the position "Senior Full Stack Engineer"
  And I select the job from the results
  And I click "Apply Now"
  Then the application page should show:
    | Job Title    | Senior Full Stack Engineer                       |
    | Job Location | Durham, North Carolina, United States of America |
    | Job Id       | 2517089                                          |
    | Job Type     | Full-Time                                        |
  When I click "Return to Job Search"
  Then I should be back on the job search results page
