# Inventory Microservice
This project is a sample product Inventory management system using micro service architecture

# Technologies
<ul>
<li>Asp.net Core Api (C#) </li>
<li>Postgres Database</li>
<li>RabbitMQ</li> 
<li>GraphQL </li> 
</ul>

# Features
- User must login before access to the system
- User must have role(InventoryKeeper, Requester or Approver)
  
  ### InventoryKeeper

  - Inventory Keeper must be able to add new item to inventory
  - Update Inventory
  - see all items in inventory and the quantity available

  ### Requester
  
  - Must be able to request only available items in inventory and see the status of the request

  ### Approver
  
  - Must be able to approve or disapprove request from a requester.

# System Testing 
  - Clone to the project or download and extract the project files.
  - Run the migrations for both IdentityService and InventoryService.
  - Setup your solution startup project to Multiple Start projects from solution properties.
  - Then set all poroject to start
  - Start the application
  - Happy Hacking
