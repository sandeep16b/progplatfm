# Name
Abim.Platform.Program
## Summary

The main project of the Program Platform is an API which utlizes several other projects which perform operations on behalf of requests sent to the API.  
This project hosts services, commands, events, handlers, external facing routes and their controllers.
There is a Repository layer which integrates with SQL Server via fluent nHibernate defined mappings.  
RabbitMQ consumers and RabbitMQ event publishing are utilized for cross platform messaging over the bus.
Interservice calls are utilized to Profile, Product, and Regstration platforms.

## Table of Contents

[[_TOC_]]

## System Overview

| Type | Name | Executable name | Available Externally |
|---|---|---|---|
| Web | Abim.Platform.Program.Host | Abim.Platform.Program.Host.exe | False |
| Web | Abim.Platform.Program.Jobs | Abim.Platform.Program.Jobs.exe | False |
### Business overview

(Topic header)  
Abim.Platform.Program.Host
What business need is met by this service or system? Who are the  main customers (diplomates, internal users, etc.)? What expectations do we have about availability and performance?

(Topic body)  
Program Platform API Host contains all API controllers related to Certifications, Credentials, VOC Physician Credentials, and Sources.

Program Platform Consumer Host contains all consumers for events published by other platorms.

Program Platform Jobs contains job queues that handle background job processing.

### Contributing applications, daemons, services   (Subsection header)

(Topic header)  
Which distinct software applications, databases, services, etc. make up the service or system? What external dependencies does it have?

(Topic body)  
Program Platform uses a SQL Server database named Certification.  

## System Characteristics 

## Required Resources

### Dependencies

* RabbitMQ
* Hangfire
* Program Database
* Identity Platform
* Connection to Interservices for Identity, Profile, Product, and Registration

### Servers
| Environment | Servers |
|---|---|
| Dev | PPUABIMAPST18; PPUABIMAPST19; PPUABIMSQLT07 |
| QA | PPUABIMAPSQ08; PPUABIMAPSQ09; PPUABIMSQLQ02 |
| Clean | PPUABIMAPSC05; PPUABIMAPSC06; PPUABIMSQLC01 |
| Production | NPUABIMAPSV26; NPUABIMAPSV27; NPUABIMAPSV28; SQLCLUSTER |

## Security and Access Control
Claims based authorization. Impersonation allows read-only access.

## System Configuration 
Standard Platform API configuration with interservices to other platforms, logging, and Identity Platform intergration.

.NET Framework 4.6.1
.NET Framework 4.5.2

###RabbitMQ Configuration
The Program Platform has several queues for processing data which may be sent from other systems:

-   program_correctiveAction_queue
-	program_fphmAttestInitial_queue
-	program_examResultRelay_queue

###Retry Policy Configurations
The Program Platform implements retry policies for Interservice Communication to prevent errors due to problems with message queuing, deadlocks or other failures. This policy can be configured with number of retries and pause in seconds between retries. 

## Hangfire Jobs
The Program Platform Worker Host uses Hangfire Dashboard to view job/job states and the ability to manually trigger defined jobs. Hangfire Dashboard uses basic authentication defined in Octopus Variables.

The following Recuring Hangfire Jobs every year:

ExpireByTimeLimitJob | triggered automatically on September 1 of each year
YearEndLookbackJob | triggered manually by AddDevs, usually at the beginning of February each year.
YearEndLookbackTestJob | triggered manually by AddDevs for testing purpose
EarlyYearEndLookbackJob | triggered automatically on January  1 of each year
DeselectCertificateJob | triggered automatically on February  1 of each year

## System Backup and Restore
Standard backup and restore procedures are in place. Previous versions of the app can be deployed via Octopus.

## Monitoring and Alerting  

Slack notifications and health check endpoints.

## Operational Tasks 
None.
## Maintenance Tasks 
None.
## Failover and Recovery Procedures
Standard. We can deploy previous builds with Octopus.