import glob
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns

percentages = np.array([0.25,0.5,0.75,1])

def main():
    global percentages
    parseTextFiles()

def parseTextFiles():
    names = glob.glob("*.txt")
    for name in names:
        f = open(name,'r')
        data = f.read().split('\n')
        gender, age, cumulativeAccuracy, cumulativeOneLevelAccuracy, accuracyByPercentage = processFile(data)
        f.close()

def processFile(data = []):

    percentagesCorrect = np.array([0.0,0.0,0.0,0.0])
    percentageCount = np.array([0,0,0,0])
    info = data[0].split(',')
    age = info[1]
    gender = info[2]
    cumulativeAccuracy = np.array([])
    cumulativeOneLevelAccuracy = np.array([])
    cleanData = data[2:len(data)-1]
    correct = 0.0
    correctByOneLevel = 0.0
    for i, entry in enumerate(cleanData):
        values = entry.split(',')
        userAnswer = float(values[1])
        correctAnswer = float(values[2])
        percentageIndexR, = np.where(percentages == correctAnswer)
        percentageIndex = percentageIndexR[0]
        percentageCount[percentageIndex] += 1
        if correctAnswer == userAnswer:
            correct += 1
            percentagesCorrect[percentageIndex] += 1
        elif abs(userAnswer - correctAnswer) == 0.25:
            correctByOneLevel += 1
        cumulativeAccuracy = np.append(cumulativeAccuracy, correct/(i+1))
        cumulativeOneLevelAccuracy = np.append(cumulativeOneLevelAccuracy, correctByOneLevel/(i+1))
    accuracyByPercentage = np.divide(percentagesCorrect, percentageCount)
    return gender,age,cumulativeAccuracy,cumulativeOneLevelAccuracy, accuracyByPercentage


if __name__ == "__main__":
    main()
