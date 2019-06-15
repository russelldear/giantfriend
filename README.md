# giantfriend

DotnetCore lambda that updates recent Strava rides to be eBike rides, with a specified bike.

The Strava watch app doesn't let me start a ride as an eBike ride - I have to remember to go and change it later. This solves that problem. 

## Required Environment Variables
- ClientId
- ClientSecret
- RefreshToken
- ActivityType
- GearId

### How to get that stuff ^^^
- You get ClientId & Client Secret when you create your app at https://www.strava.com/settings/api
- ActivityType values are listed here: https://developers.strava.com/docs/reference/#api-models-ActivityType - I use EBikeRide
- GearId - go to https://www.strava.com/settings/gear and click on the bike you want. The id is in the url - prefix it with 'b' and that's your gear id. Alternatively, run the lambda after you've manually updated an activity to use your selected bike; the `gear_id` (and a whole bunch of other stuff) will be spat out in the Cloudwatch logs.

- RefreshToken requires a few steps:
  - Navigate to the following url in your browser, with the appropriate ClientId replacement: https://www.strava.com/api/v3/oauth/authorize?client_id={CLIENT_ID}&redirect_uri=http://localhost&response_type=code&scope=read_all,activity:read,activity:write
  - Once you're redirected, copy the code out of the url in the address bar.
  - In Postman or similar, make a POST to https://www.strava.com/api/v3/oauth/token?client_id={CLIENT_ID}&client_secret={CLIENT_SECRET}&code={CODE_FROM_THE_PREVIOUS_STEP}&grant_type=authorization_code

You'll get the RefreshToken for your environment variables in the reponse from that post.

# Deployment
- Create a lambda in the AWS console with the above environment variables
- Run `dotnet lambda package` in the .csproj directory
- Upload the resulting zip to your new lambda
- ????
- Profit!

AWS CodeBuild will automate this for you on push - just point it at the buildspec.yml file.
