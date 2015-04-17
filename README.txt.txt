Implementation.
The method createRandomMap() is called to generate a randomly seeded height map.
This map is generated using the Diamond Square Algorithm.
This height map is stored in a 2D array.  Next the vertex normals are calculated
by for each (x,y) point in the height map.  They take the points to the left,
right, top and bottom of the point (x,y) to calculate the slopes along the x
and z axis and generate a normal using the cross product.
The vertices are then added to the vertices list as triangles. Two triangles
are generated to draw the sqare for the height map points of (x,y) (x+1,y)
(x+1,y+1) and (x,y+1).
Lighting is applied using BasicEffect and BlendState using the calculated 
vertex normals. Colors are assigned to points based on their height map values.
The lighting direction is also changed according to trigonmetric functions 
using the GameTime.
The Camera is it's own object that can fly around the landscape. 
Input is read from keyboard and mouse.
Changes are then applied to translation of camera's position and rotation of 
its view direction. 
Finally these changes are checked for collisions and adjusted as needed

Controls.
W - Move Forward
A - Move Left
S - Move Back
D - Move Right
Q - Move Up
E - Move Down
Left - Rotate Left
Up - Rotate Up
Down - Rotate Down
Right - Rotate Right
Mouse - Rotate (Y-Axis not inverted)
ESC - Exit Game

Resources.
The implementation of the diamond square algorithm was done with help from the
java implementation found at: 
http://stackoverflow.com/questions/2755750/diamond-square-algorithm

The implementation of applying transformation changes with the camera was done
with help from the code sample found at:
https://code.google.com/p/sharpdx-examples/source/browse/SharpExamples/SharpExamples/CameraFirstPerson.cs?r=af1ea7af8fd808495d675d3a5aada6b82c19b9c9
