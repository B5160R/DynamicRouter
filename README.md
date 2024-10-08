# Dynamic Routing in RabbitMQ (Proof of Concept)
Meant to illustrate dynamic routing in RabbitMQ using a combination of exchanges, queues, and routing keys. This proof of concept demonstrates how messages can be dynamically routed to different consumers based on routing keys, and how new consumers can be set up to listen to specific routing keys without modifying the existing infrastructure.

## Steps to run:

-  Start RabbitMq in Docker:

    `docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management`
- Start Producer & Consumer in seperate terminals
- Follow instructions 