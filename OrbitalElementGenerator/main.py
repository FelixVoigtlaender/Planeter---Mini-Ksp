import json
import orbital
from orbital import KeplerianElements


def write_json(data, filename):
    with open(filename, "w") as f:
        json.dump(data,f,indent=4)

def generatePlanet(name, center, a_semiMajorAxis, e_eccentricity, mass, radius):
    print(f'{name}: Center:{center} semiMajorAxis: {a_semiMajorAxis}, eccentricity: {e_eccentricity}, mass: {mass}, radius {radius}')

    #Data to be written
    orbitElement = {
        "name" : name,
        "center" : center,
        "a_semiMajorAxis" : a_semiMajorAxis,
        "e_eccentricity" : e_eccentricity,
        "mass" : mass,
        "radius" : radius
    }
    return orbitElement

def generatMoon(name, center, a_semiMajorAxis, e_eccentricity, mass, radius):
    print(f'{name}: Center:{center} semiMajorAxis: {a_semiMajorAxis}, eccentricity: {e_eccentricity}, mass: {mass}, radius {radius}')

    #Data to be written
    orbitElement = {
        "name" : name,
        "center" : center,
        "a_semiMajorAxis" : a_semiMajorAxis,
        "e_eccentricity" : e_eccentricity,
        "mass" : mass,
        "radius" : radius
    }
    return orbitElement



#Planets
#planet = = generatePlanet("Name","Center",semiMajoAxis,eccentricity,mass,radus)
mercury = generatePlanet("Mercury", "Sun",  0.39,   0.206,  0.06,   0.383)

venus = generatePlanet("Venus",     "Sun",  0.72,   0.007, 0.81,   0.949)

earth = generatePlanet("Earth",     "Sun",  1.00,   0.017, 1.00	,   1.000)
moon = generatMoon("Moon",     "Earth",  384400,   0.0554, 1.00	,   1.000)


mars = generatePlanet("Mars",     "Sun",  1.52,   0.093, 0.11	,   0.532)

moon = generatMoon("Phobos",     "Mars",  9376,   0.0151, 1.00	,   1.000)
deimos = generatMoon("Deimos",     "Mars",  23458,   0.0002, 1.00	,   1.000)

#Jupiter
jupiter = generatePlanet("Jupiter", "Sun",  5.20	,   0.048, 317.83,   11.209)
#Jupiter Moons
io = generatMoon("Io",              "Jupiter",  421800,  0.0094, 1.00	,   1.000)
europa = generatMoon("Europa",      "Jupiter",  671100,   0.0013, 1.00	,   1.000)
ganymede = generatMoon("Ganymede",  "Jupiter",  1070400,   0.0013, 1.00	,   1.000)
callisto = generatMoon("Callisto",  "Jupiter",  1882700,  0.0074, 1.00	,   1.000)
amalthea = generatMoon("Amalthea",  "Jupiter",  181400,   0.0032, 1.00	,   1.000)
thebe = generatMoon("Thebe",        "Jupiter",  221900,   0.0176, 1.00	,   1.000)
adrastea = generatMoon("Adrastea",  "Jupiter",  129000,   0.0018, 1.00	,   1.000)
metis = generatMoon("Metis",        "Jupiter",  128000,   0.0012, 1.00	,   1.000)

saturn = generatePlanet("Saturn",  "Sun",  9.54	,   0.054, 95.16,  9.449)

uranus = generatePlanet("Uranus",   "Sun",  19.19	,   0.047, 14.54,   4.007)

neptune = generatePlanet("Neptune", "Sun",  30.07   ,   0.009, 17.15,   3.883)

planets = [mercury, venus, earth, mars, jupiter, saturn, uranus, neptune]

#Moons
#planet = = generatePlanet("Name","Center",semiMajoAxis,eccentricity,mass,radus)


data = {"planets" :  planets}
write_json(data,"Planets.json")

