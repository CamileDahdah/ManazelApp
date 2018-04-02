# Technical Documentation

# Overview

Manazel is an app that helps children with speech disabilities to pronounce words correctly. The user should guess objects present in the room (or level) in order to earn points and proceed to the next level.

Main Features

The app uses Google non-streaming speech recognition to detect user&#39;s voice. It also uses 360 panorama technology to display room&#39;s views. Consequently, each view has its own texture.

The user can either look around the room by swiping with his fingers or by rotating the device using the accelerometer. To interact with the objects in the room, he can simply click them (touch them). The object will then appear with a simple rotating sprite animation.

# Implementation

#### Speech Recognition

 I used my Google Cloud account in order to use Google non-streaming speech recognition API. First, I enabled the service and then retrieved my free trial API key to include it in my HTTP request header.

The API call requires four main parameters in the JSON payload:

1. Language (i.e. Lebanese Arabic)
2. Audio encoding: Linear16 (Standard) or other
3. Sample Rate Hertz (for audio sampling)
4. Audio data in 64-bit encoding format

 I used SavWav library in able to save my recorded audio in WAV format.

 The audio recorder automatically stops recording when the user stops talking. It also detects the sound level of the environment in order to guess when the user is actually talking. In unity, this feature was hard to implement because you have to play the recorded audio source (which should be muted) while recording it to be able to get the amplitude of the sound.





#### Room View Manager

 I used prefabs to instantiate room views, objects, hotspots, etc. dynamically. All the prefabs are located in the Resources folder in order to load them dynamically. They are organized in this hierarchy:

 1. Resources/Room/RoomName

   a. Hotspots
   
    1. i)View 1
    2. ii)View 2
    3. iii)View 3
    4. iv)Till View 6
    
   b. Objects
   
    1. i)View 1 till View 6
    
   c. Textures (Views)
   
    1. i)View 1 till View 6
    
   d. Etc.
   
    1. i)View 1 till View 6



#### Object Manager

 Objects are represented in a JSON file called &quot;Objects.json&quot; and each JSON object is structured in this way:

{

"id":&quot;Sofa&quot;,

&quot;lbArabicWord&quot;:&quot;كنب&quot;,

&quot;saudiArabicWord&quot;:&quot;كنب&quot;,

}

The ID of the object is used for many purposes, including a reference to its sprite animation located in path: Resources/Sequences/&quot;ObjectID&quot;.

#### Arabic Support

 Unity Does not support Arabic text so I used a library called &quot;ArabicFix&quot; to attach and reverse the order of Arabic letters.

#### Writing to Objects.json file in Android

 Since Resources folder is read-only in Android (I still haven&#39;t checked for IOS), I had to copy the file form Resources folder to Application.persistentDataPath when the application is run for the first time. Hence, all the write/read operations are then applied to the new file that was created on run-time.

#### Colliders to objects in scene

 First, we were creating the colliders manually in Unity so that we can detect the rendered objects on the textures. Then, our 3D designer suggested to generate low poly colliders to all the objects for each view automatically. His approach was much more efficient to implement.

Note that I haven&#39;t implemented his work yet, but we should implement it as soon as possible.

#### Attached scripts

 Scripts are attached to each hotspot in the scene in order to manually assign a view ID for it in the inspector. Same goes for objects&#39; colliders, a script is attached to the parent game object which contains its object ID in the inspector.

#### Git Version Control, Google Docs, and Trello

 We are using BitBucket to manage our Git repository. I am personally using GitKraken as a Git GUI client.

All issues and documents are shared on Google Docs to manage our tasks. We are also using Trello to organize them.

#### Code Design

 I followed C# coding design as much as possible to avoid irregularities. I wrote comments in places where the logic behind the code may not be very clear. Some classes were written in a hurry; hence, their design might change afterwards.

 I used TODO comments to notify the developer about implementations that should be done or modified in the scripts. I highly recommend you to use the IDE to keep track of these special comments.