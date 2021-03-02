import json
import orbital
from orbital import KeplerianElements


#https://ssd.jpl.nasa.gov/?sat_elem#earth

def write_json(data, filename):
    with open(filename, "w") as f:
        json.dump(data,f,indent=4)

def generatePlanet(name, center, a_semiMajorAxis, e_eccentricity, mass, radius, color):
    print(f'{name}: Center:{center} semiMajorAxis: {a_semiMajorAxis}, eccentricity: {e_eccentricity}, mass: {mass}, radius {radius}')

    #Data to be written
    orbitElement = {
        "name" : name,
        "center" : center,
        "a_semiMajorAxis" : a_semiMajorAxis,
        "e_eccentricity" : e_eccentricity,
        "mass" : mass,
        "radius" : radius,
        "color" : color
    }
    return orbitElement

def generateMoon(name, center, a_semiMajorAxis, e_eccentricity, mass, radius, color):
    print(f'{name}: Center:{center} semiMajorAxis: {a_semiMajorAxis}, eccentricity: {e_eccentricity}, mass: {mass}, radius {radius}')

    a_semiMajorAxis /= 149598023
    mass /= 5.97237 * E(24)
    radius /= 6371.0
    #Data to be written
    orbitElement = {
        "name" : name,
        "center" : center,
        "a_semiMajorAxis" : a_semiMajorAxis,
        "e_eccentricity" : e_eccentricity,
        "mass" : mass,
        "radius" : radius,
        "color" : color
    }
    return orbitElement
def E(n):
    return  pow(10,n)


planets = []
moons = []
#generateMoon(name, center, a_semiMajorAxis, e_eccentricity, mass, radius, color):
#Sun
planets.append(generatePlanet("Sun", "Sun", 0,   0,  33300,   109,"#ffc700"))

#Mercury
planets.append(generatePlanet("Mercury", "Sun",  0.39,   0.206,  0.06,   0.383,"#cbcaff"))

#Venus
planets.append(generatePlanet("Venus",     "Sun",  0.72,   0.007, 0.81,   0.949, "#ff751e"))

#Earth
planets.append(generatePlanet("Earth",     "Sun",  1.00,   0.017, 1.00	,   1.000,"#5e74ff"))
moons.append(generateMoon("Moon", "Earth", 384399,0.0549, 7.342*E(22), 1737.4, "#c0c0c7"))

#Mars
planets.append(generatePlanet("Mars",     "Sun",  1.52,   0.093, 0.11	,   0.532, "#e50101"))
moons.append(generateMoon("Phobos", "Mars", 9376, 0.0151, 1.0659*E(16), 11.2667, "#c0c0c7"))
moons.append(generateMoon("Deimos", "Mars", 23458, 0.0002, 1.4762*E(15),6.2, "#c0c0c7"))

#Jupiter
planets.append(generatePlanet("Jupiter", "Sun",  5.20	,   0.048, 317.83,   11.209, "#fd2812"))
#moons.append(generateMoon("Io", "Jupiter", 421800, 0.0094, 1.00, 1.000, "#c0c0c7"))
#moons.append(generateMoon("Europa", "Jupiter", 671100, 0.0013, 1.00, 1.000, "#c0c0c7"))
#moons.append(generateMoon("Ganymede", "Jupiter", 1070400, 0.0013, 1.00, 1.000, "#c0c0c7"))
#moons.append(generateMoon("Callisto", "Jupiter", 1882700, 0.0074, 1.00, 1.000, "#c0c0c7"))
#moons.append(generateMoon("Amalthea", "Jupiter", 181400, 0.0032, 1.00, 1.000, "#c0c0c7"))
#moons.append(generateMoon("Thebe", "Jupiter", 221900, 0.0176, 1.00, 1.000, "#c0c0c7"))
#moons.append(generateMoon("Adrastea", "Jupiter", 129000, 0.0018, 1.00, 1.000, "#c0c0c7"))
#moons.append(generateMoon("Metis", "Jupiter", 128000, 0.0012, 1.00, 1.000, "#c0c0c7"))

#Saturn
planets.append(generatePlanet("Saturn",  "Sun",  9.54	,   0.054, 95.16,  9.449, "#f7dbb9"))
#generateMoon(name, center, a_semiMajorAxis, e_eccentricity, mass, radius, color):
#moons.append(generateMoon("Mimas", "Saturn", 185539, 0.0196, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Enceladus", "Saturn", 238042, 0.0000, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Tethys", "Saturn", 294672, 0.0001, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Dione", "Saturn", 377415, 0.0022, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Rhea", "Saturn", 527068, 0.0002, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Titan", "Saturn", 1221865, 0.0288, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Hyperion", "Saturn", 1500933, 0.0232, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Iapetus", "Saturn", 3560854, 0.0293, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Phoebe", "Saturn", 12947918, 0.1634, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Janus", "Saturn", 151450, 0.0098, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Epimetheus", "Saturn", 151450, 0.0161, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Helene", "Saturn", 377444, 0.0000, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Telesto", "Saturn", 294720, 0.0002, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Calypso", "Saturn", 294721, 0.0005, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Atlas", "Saturn", 137774, 0.0011, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Prometheus", "Saturn", 139429, 0.0022, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Pandora", "Saturn", 141810, 0.0042, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Pan", "Saturn", 133585, 0.0000, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Methone", "Saturn", 194402, 0.0000, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Pallene", "Saturn", 212282, 0.0040, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Polydeuces", "Saturn", 377222, 0.0191, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Daphnis", "Saturn", 136504, 0.0000, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Anthe", "Saturn", 196888, 0.0011, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Aegaeon", "Saturn", 167425, 0.0002, mass, radius, "#c0c0c7"))


#Uranus
planets.append(generatePlanet("Uranus",   "Sun",  19.19	,   0.047, 14.54,   4.007, "#70fcfd"))
#moons.append(generateMoon("Ariel", "Uranus", 190900, 0.0012, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Umbriel", "Uranus", 266000, 0.0039, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Titania", "Uranus", 436300, 0.0011, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Oberon", "Uranus", 583500, 0.0014, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Miranda", "Uranus", 129900, 0.0013, mass, radius, "#c0c0c7"))

#Neptune
planets.append(generatePlanet("Neptune", "Sun",  30.07   ,   0.009, 17.15,   3.883, "#6bfffe"))
#moons.append(generateMoon("Triton", "Neptune", 0.0000	, 0.0013, mass, radius, "#c0c0c7"))
#moons.append(generateMoon("Nereid", "Neptune", 0.7507, 0.0013, mass, radius, "#c0c0c7"))

data = {"planets" :  planets,
        "moons" : moons}
write_json(data,"Planets.json")

