# Antymology
 

![Ants](Images/Ants.gif)

## Goal
Ants can display impressive emergent properties. One of which is nest building. The aim of this program is to simulate how ants can build such nests by utlising a genetic algorithm in order to breed ants that are more optimised to building bigger nests.

## World
Their are 2 types of entities that are present in the world Ants and Blocks. Both entites are represented as a cube inside the simulation ,however, each entity and subentity types have their own color (materials in unity) to distinguish between them inside the simulator
### Blocks
Blocks are static entites in the world. They do nothing except existing so that an ant can interact with them. The following are the types of blocks in the simulator:
- Mulch Blocks: can be eaten by ants to restore some of there health
- Acidic Block: Ants standing on an acidic block recieve double damage
- Air Block: these Are empty transparent block that indicate that this position in the world is empty
- Stone Block: A stone Block
- Container Block: A block that cannot be dug up

### ANTS
ANTS are the dynamic entites in the world. There are two possible types of ants: worker Ants(represented by black blocks) and the Queen Ant(represented by a golden block). Each ant starts with a specified amout of health which decreases with time and also depending on the enviroment. If an ants health reaches zero it dies and is removed from the simualtro
#### Ant Behaviour
- Ants can eat a mulch block to regain 1/3 of their starting hp. They can only do so when they are the only ant standing atop the mulch block. both Worker ants and the queen ant   can eat mulch blocks. Eating a mulch block removes it from the world.
- Ants can move through out the world by jumping onto diffrent blocks. ANTs choose whcih block to move to randomly from one of their surrounding blocks. Surrounding blocks are blocks that are 2 units or closer in height difference and 10 units or closer in each of the x and z directions. Both Worker Ants and Queen Ants move this day. 
- Ants can dig up the block that is directly underneath them as long as that block is not of type Container block. Digging up a block removes it from the world. Both worker and queen ants can do this.
- If an ant is standing on an acidic block the rate its health decreases is doubled. Applies to both Worker and Queen Ants
- Ants may give some of their health to other ants occupying the same space. Only worker ants can share their health but the queen ant is capable of recieveing health from the workers. There is no limit to how much health an ANT can have. Some ants may even kill themselves while doing this. 
- Only The Queen Ant can produce nest blocks. However, doing so reduced her health by 1/3rd of her starting health. The queen ant will not attempt to place a nest block if doing so would kill her, however she can still die from normal health decrease.

#### ANT genes
Ant genes are defined by integers that control the chance an ant will preform a specific behaviour. The following are the diffrent genes an ant has
 - health : how much health the ant has
 - healthSharePercentage: the chance the ant will share its health with another worker
 - queenhealthSharePercentage: the chance the ant will share its health with the queen
 - eatChance: the chance an ant will eat a munch block
 - digchance: the chance an ant will dig a block
 - CreatNestChance: the chance the queen will build a nest block
In additon each ant has a bool field which indicates if it is a queen. Their can only be one queen so this will
only be set once.

The value of these integers are shared between the whole population.

## Training Proccess
The ANT genes are optimised using a genetic algorithm. In the training proccess the whole ant population is treated as an individual. The training proccess is the following:
   - Generate N random individuals (note that an individual here is actually a full ant colony) . N can be changed from the controls menu (shown later)
   - for each individual :
       - Generate a new world
       - let the indivdual interact with the world for S seconds. S can be changed from the controls menu (shown later)
       - After S seconds have passed store the number of NestBlocks the individual has generated and move on to the next Individual 
   - After all indivudals in a generation are tested keep the top half scoring individuals and discard the lower half
   - generate N/2 new individuals by crossbreeding and mutating the surviving individuals. Crossbreeding is guarnteed to happen but the mutation chance in a new individual m can     be changed in the control panel
   - repeat the above for required number of Iterations i. i can be changed in the controls panel


### Controls
#### Camera Controls
-press the W key to move forward
-press the S key to move backward
-press the A key to move the the left
-press the D key to move to the right
-press Q key to move downward
-press the E key to move upward
#### UserInterface






## Citation
The world building code in this simulator was provided by Dr. Davies Cooper, all code releated to ant behaviour and the evolutionary algorithm was written by me
