## Userfull commands 

$ dotnet build 
$ dotnet run 

$ dotnet ef migrations add "name of the migration"
$ dotnet ef database update


## Security 

Web API security 

https://stormpath.com/blog/token-authentication-asp-net-core
https://github.com/nbarbettini/SimpleTokenProvider


## API Usage

baseurl = "flivedev.azurewebsites.net"

### Registration

### Register with username/password
[POST] https://{baseurl}/api/useraccount/register
{"Email":"test@test.com","Password":"Rewq4321$#@!","ConfirmPassword":"Rewq4321$#@!","UserType":"1",DeviceToken:"","Platform":"Apple","Name":"Your name"}

//note that the password need to have the standard complexity requirements. length > 6 , Numeric, Both Upper and lowercase letters , special chars


### Register with facebook
[POST] https://{baseurl}/api/useraccount/registerwithfacebook
[Headers] Authorization Bearer token-from-facebook
{"Email":"test@test.com","FacebookToken":"token-from-facebook","UserType":"1",DeviceToken:"","Platform":"Apple","Name":"Your name"}

//note that the password need to have the standard complexity requirements. length > 6 , Numeric, Both Upper and lowercase letters , special chars

### Login
[POST] https://{baseurl}/api/token
x-www-form-urlencoded

"username" = "test@test.com"
"password" = "Rewq4321$#@!"

### Subsequent requests
For all subsequent requests the following two headers need to be used

Content-Type = "application/json"
Authorization = Bearer token-received-from-login-request or the one from Facebook



### Stream

[POST] https://{baseurl}/api/stream/create
 - creates a stream with wowza and returns a json with all the information related to new stream

[GET] https://{baseurl}/api/stream/r1231998 
 - r1231998 is the streamId returned from create call

[POSt] https://{baseurl}/api/stream/start/r1231998 
 - initiates a start call to wowza


Push notifications setup

Follow the steps in  https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-ios-apple-push-notification-apns-get-started 

Upload to azure in the "Production" mode 

DeviceToken (black iphone 6) : 541e0fd0c9eb40d46ae9a925a9052eddd67a317f984cc4b2588b0b513500c1fb
DeviceToken (rohan's iphone) : f7ce7690cb412a9eb7e2eac7316fb4195cb6b5a3ad3f8ac438dd3f02b4798fbe
DeviceToken (white iphone 6) : d815be96cdb226bc6a05309873dfa1ed6c6592e4b491c43c9fecdb466cad2e8c


check this out for FB login 

http://stackoverflow.com/questions/38945996/can-i-verify-a-facebook-jwt-token-created-using-native-ios-sdk-with-a-net-backe

Signalr 

https://radu-matei.github.io/blog/aspnet-core-mvc-signalr/



Stripe Clarifications 


[09:21] <nickelback> hi guys, i'm developing a system where i want to charge a customers 'per session' basis ( imagine like Uber) .
[09:21] <nickelback> I'm not sure how this to be done
[09:22] <@hpar> are you paying multiple drivers/service providers like uber?
[09:22] <@hpar> you'll want to check out Connect https://stripe.com/docs/connect/
[09:22] <nickelback> nope.. the payments to be received in one single account.. but customers need to be charged 'per use' basis
[09:22] <nickelback> it is not really a subscription and nor once off charge
[09:23] <nickelback> in the documentation i couldn't find a reference to do this so far
[09:23] <@hpar> ah, in that case it's much simpler... collect the card number and create a customer, then create charges against the customer
 as needed https://stripe.com/docs/charges#saving-credit-card-details-for-later
[09:25] <nickelback> @hpar , thanks seems like what im after ..
[09:27] <nickelback> just one last question.. my mobile app is going to be based on ionic. do you think i should use stripejs or checkout ?
[09:27] <@hpar> you'll only be able to use stripejs with ionic, Checkout needs an actual mobile browser to work


## Image Resize 

http://jameschambers.com/2016/11/Resizing-Images-Using-Azure-Functions/

Au

Deployment steps 

1. Patch database
2. Deploy Web app 
3. Register as a new user 
4. Generate auth token with 5 year validity 
5. Update Webjob config with new token 
6. Deploy webjob
7. Change the Webjob Auth credentials in Azure
8. Update the app version 
9. Build the pacakge 


Production Deployment Steps 

1. Patch database
2. Change the Stripe production credentials in both WebJob and WebApp
3. Deploy the web app 
4. Deploy the WebJob
5. Update the version 
6. Make the package
7. Change the categories ( get the new yoga image from Rohan)
8. Update the T&Cs
