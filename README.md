# Bemagine Service Model

The Bemagine Service Model project, like many preceding, arose out of the necessity to address a problem domain not addressed
an existing solution. The need in this case, was for a generic WCF channel supporting the JMS messaging protocol that provided
a mechanism for draining JMS queues based on available resources. WCF does support internally a dispatch mechanism that dispatches
request locally as local resources become available, but at that point, the message was already drained from the JMS queue. 
Consider the scenario where multiple consumers can process messages from a given queue, but each message requires a non-uniform
distribution of running times with a high degree of variance. The messages taxonomically may be homogenous, as for example, 
would be the case of a fixed income analytics engine consuming calculation requests. If N calculation requests were queued 
simultaneously with two consumers of that queue, for the sake of simplicity let's assume they drain the queue in a round
robin manner, the consumers may drain the JMS queue into local WCF dispatch. This behavior was observed with some vendor 
implementations of a JMS WCF Channel. The problem, continuing with our example of the fixed income analytic engine is that 
the total running time for each consumer will exhibit a degree of variance with respect to the population of consumed messages.
In this case, one consumer, by chance, may drain mostly short duration tasks and the other long duration, leaving that consumer
idle for some period of time while the other toils. What if, we throttled the consumption of messages from the JMS queue such
that each consumer processed only that number of messages for which they had resources available (i.e. the local dispatch
queue of any given consumer would have zero size). Such throttling, per se, has the benefit of reducing the overall running 
time to something equivelent to the sum of running times of all the tasks because no consumer would idle. That was the driving
impetus for this project. Overtime, additional features arose, and I will have to describe them in some detail at later date, 
but for the most part, there's sufficient commentary in the code to provide context.
