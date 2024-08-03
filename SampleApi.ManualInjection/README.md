# Multi-tenant API example (without DI)

This is what Developers often do When they need to inject async Service on Runtime, but don't know how to do it.

Because they don't know how to make the DI framework does the job, they would simply stop using the DI framework, and so manually write "new" statement to create objects whenever needed.

Checkout other solution: [`SampleApi.AsyncInjection` project](../SampleApi.AsyncInjection/).
