# Shuttle.Esb.EMail

An e-mailing solution built on Shuttle.Esb that makes use of SMTP for sending mails with optional attachments.

## Sending an e-mail request

Send a `SendEMailCommand` to an endpoint that will perform the e-mail communication.

## IEMailAttachmentService

``` c#
SendEMailCommand.Attachment AddAttachment(SendEMailCommand command, string path);
```

Adds an attachment to a `SendEMailCommand` message.  The `path` will be a file that typically exists on the sending machine.

The remaining methods would not usually be used by a developer or client code.

## Shuttle.Esb.EMail.Server

A server endpoint should exist that perform that actual SMTP communication.  To this end the following configuration would need to be provided:

``` xml
<?xml version="1.0" encoding="utf-8"?>

<configuration>
  <configSections>
    <section name="email" type="Shuttle.Esb.EMail.Server.EMailSection, Shuttle.Esb.EMail.Server" />
    <section name="serviceBus" type="Shuttle.Esb.ServiceBusSection, Shuttle.Esb" />
  </configSections>

  <appSettings>
    <add key="AttachmentFolder" value="" />
  </appSettings>

  <email
    host=""
    port="25"
    useDefaultCredentials="true"
    username=""
    password=""
    domain=""
    enableSsl="false"
    trackerExpiryInterval="00:00:15"
    trackerExpiryDuration="08:00:00" />

  <serviceBus>
    <inbox
      workQueueUri=""
      deferredQueueUri=""
      errorQueueUri="" />
  </serviceBus>
</configuration>
```

### email

| Attribute | Default | Description |
| --- | --- | --- |
| host | n/a | The SMTP server to use. |
| port | 25 | The port over which to communicate with the SMTP server. |
| useDefaultCredentials | `true` | Set to `true` to use the credentials of the process identity; else `false` to provide custom, or empty, credentials. |
| username | `""` | The username used for authentication. |
| password | `""` | The password used for authentication. |
| domain | `""` | The domain used for the credentials. |
| enableSsl | false | Set to `true` to enable communication over SSL; else `false`. |
| trackerExpiryInterval | `"00:00:15"` | A `TimeSpan` interval indicating how often to call the `IEMailTracker` implementation to expire delivered e-mails. |
| trackerExpiryDuration | `"08:00:00"` | A `TimeSpan` duration that specifies how long a delevired e-mail will be tracked before being removed. |

### serviceBus

| Attribute | Default | Description |
| --- | --- | --- |
| workQueueUri | n/a | Specify the `uri` to use as the inbox work queue. |
| deferredQueueUri | n/a | Specify the `uri` to use as the inbox deferred queue. |
| errorQueueUri | n/a | Specify the `uri` to use as the inbox error queue. |

### Queue configuration

Since the `Shuttle.Esb.EMail.Server` does not include any specific queue implementation out-of-the-box you would need to drop the relevant assemblies into the deployment folder.  

If you do not have the assemblies readily available from some cache, such as the Nuget cache, then you could perform somewhat of a *faux*-install of the relevant package and copy the relevant assemblies from there:

```
nuget install Shuttle.Esb.RabbitMQ -OutputDirectory packages
```

Here the `Shuttle.Esb.RabbitMQ` package will be extracted to the `packages` folder along with all the relvant dependencies.