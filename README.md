# SimpleStart - Object Generator

I've always been intrigued by Unity asset store tools that take a simple problem and automate it to speed a developers workflow within a project. This inspired me to create a simple probability based object generator for games development. I am currently developing a mobile game that involves recycling and creating a lot of grid based isometric levels and rather then manually duplicated each tile I needed, I created this tool to solve the issue and give me all the desired objects in the correct quantity in one click.

Here is a link to the asset store for further information and screenshots: 	https://www.assetstore.unity3d.com/#!/content/106509


Below is an extract taken from the ReadMe I have included within the asset that offers a very detailed overview of the product
and its functionality:

---------------------------------------------------------------------------------------------------------------------------------
Introduction					      	
---------------------------------------------------------------------------------------------------------------------------------

Thank you for downloading SimpleStart - Object Generator.

The purpose of this asset is to give new and existing projects a very lightweight
and quick to set up script for probability based object generation. In addition to this 
there is a feature to also place the objects in a uniform grid after generation.

Current Version: 1.0.0

---------------------------------------------------------------------------------------------------------------------------------
The Files					      	
---------------------------------------------------------------------------------------------------------------------------------

The essential components to this asset are:

	ObjectGenerator.cs - The script responsible for all the logic the asset handles
	ObjectGeneratorEditor.cs - The custom editor to allow easy use of the asset within the inspector

The rest of the included assets are included for use in the example scene, which shows the asset in use.

A brief summary of these assets:

	Tile Scripts: A tile interface and 4 different tile scripts that implement this interface.
	Tile objects: 4 different tile objects with their associated scripts attached 
	Tile Prefab: A gameobject for generation with each different tile game object nested and deactivated
	Materials: Different colour materials for each example tile game object:
	Scene 1: The object generator set up with the tile prefab and ready to be used (read guide below).
	Scene 2: The result of the object generator (read guide below).
