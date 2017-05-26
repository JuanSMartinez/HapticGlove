#!/usr/bin/env python
# -*- coding: utf-8 -*-
'''
Data analysis script
'''
import glob
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns

#percentages to work with
PERCENTAGES = np.array([0.25, 0.5, 0.75, 1])

#Male culumative accuracies matrix
MCAM = np.array([])

#Male cumulative accuracies by one level matrix
MCOAM = np.array([])

#Female cumulative accuracies matrix
FCAM = np.array([])

#Female cumulative accuracies by one level matrix
FCOAM = np.array([])

#Full matrices
FULLCAM = np.array([])
FULLCOAM = np.array([])

#Percentage accuracies
PACCM = np.array([])

#String titles and labels for graphs
ACC_ONE_LEVEL_TITLE = "Porcentaje Acumulado de Aciertos\ndentro de un Nivel de Diferencia"
ACC_TITLE = "Porcentaje Acumulado de Aciertos"
Y_LABEL = "Porcentaje de Aciertos (%)"
X_LABEL = "Intento"

#Name of file with total statistics
FILE_NAME = "averages.dat"

def main():
    '''Main method'''
    parse_text_files()
    append_matrices()
    analyze()

def parse_text_files():
    '''Query, read and process the text files'''
    names = glob.glob("*.txt")
    for name in names:
        text_file = open(name, 'r')
        data = text_file.read().split('\n')
        gender, age = get_gender_age(data)
        cumulative_acc, cumulative_one_level_acc, acc_by_percentage = process_file(data)
        append_matrices_gender(gender, cumulative_acc, cumulative_one_level_acc, acc_by_percentage)
        text_file.close()

def analyze():
    '''Analyze and plot all the data'''
    global ACC_ONE_LEVEL_TITLE, ACC_TITLE, Y_LABEL, X_LABEL, FILE_NAME
    #Plots
    #plot_f_statistics(show=False)
    #plot_m_statistics(show=False)
    #plot_full_statistics(show=False)
    plot_full_statistics_s(show=False)

    #Statistics
    female_accuracy_avg = np.mean(FCAM[:, FCAM.shape[1]-1].flatten(), dtype=np.float32)
    female_accuracy_avg_one_level = np.mean(FCOAM[:, FCOAM.shape[1]-1].flatten(), dtype=np.float32)
    male_accuracy_avg = np.mean(MCAM[:, MCAM.shape[1]-1].flatten(), dtype=np.float32)
    male_accuracy_avg_one_level = np.mean(MCOAM[:, MCOAM.shape[1]-1].flatten(), dtype=np.float32)
    total_accuracy_avg = np.mean(FULLCAM[:, FULLCAM.shape[1]-1].flatten(), dtype=np.float32)
    total_accuracy_avg_one_level = np.mean(FULLCOAM[:, FULLCOAM.shape[1]-1].flatten(), dtype=np.float32)
    percentage_accuracies_avg = np.mean(PACCM, axis=0, dtype=np.float32)

    #File writing
    out = open(FILE_NAME, 'w')
    out.write("Average female accuracy : " + str(female_accuracy_avg) + "%\n")
    out.write("Average female accuracy by one level : " + str(female_accuracy_avg_one_level) + "%\n")
    out.write("Average male accuracy : " + str(male_accuracy_avg) + "%\n")
    out.write("Average male accuracy by one level : " + str(male_accuracy_avg_one_level) + "%\n")
    out.write("Total average accuracy : " + str(total_accuracy_avg) + "%\n")
    out.write("Total average accuracy by one level : " + str(total_accuracy_avg_one_level) + "%\n")
    out.write("25% level average accuracy : " + str(percentage_accuracies_avg[0]) + "%\n")
    out.write("50% level average accuracy : " + str(percentage_accuracies_avg[1]) + "%\n")
    out.write("75% level average accuracy : " + str(percentage_accuracies_avg[2]) + "%\n")
    out.write("100% level average accuracy : " + str(percentage_accuracies_avg[3]) + "%\n")
    out.close()


def plot_full_statistics_s(show=True):
    '''Plot separate full statistics'''
    plt.figure(figsize=(14.3, 7.7))
    plot_matrix(FULLCAM, Y_LABEL, X_LABEL, ACC_TITLE)
    plt.tight_layout()
    if show:
        plt.show()
    else:
        plt.savefig("full_acc.eps")

    plt.figure(figsize=(14.3, 7.7))
    plot_matrix(FULLCOAM, Y_LABEL, X_LABEL, ACC_ONE_LEVEL_TITLE)
    plt.tight_layout()
    if show:
        plt.show()
    else:
        plt.savefig("full_acc_one.eps")

def plot_full_statistics(show=True):
    '''Plot the full statistics'''
    plt.figure(figsize=(14.3, 7.7))
    plt.subplot(121)
    plot_matrix(FULLCAM, Y_LABEL, X_LABEL, ACC_TITLE)
    plt.subplot(122)
    plot_matrix(FULLCOAM, Y_LABEL, X_LABEL, ACC_ONE_LEVEL_TITLE)
    plt.suptitle(u"Estadísticas Totales", fontsize=18)
    plt.tight_layout()
    plt.subplots_adjust(top=0.86)
    if show:
        plt.show()
    else:
        plt.savefig("full.eps")

def plot_f_statistics(show=True):
    '''Plot the female statistics'''
    plt.figure(figsize=(14.3, 7.7))
    plt.subplot(121)
    plot_matrix(FCAM, Y_LABEL, X_LABEL, ACC_TITLE)
    plt.subplot(122)
    plot_matrix(FCOAM, Y_LABEL, X_LABEL, ACC_ONE_LEVEL_TITLE)
    plt.suptitle(u"Estadísticas de Mujeres", fontsize=18)
    plt.tight_layout()
    plt.subplots_adjust(top=0.86)
    if show:
        plt.show()
    else:
        plt.savefig("female.eps")

def plot_m_statistics(show=True):
    '''Plot the male statistics'''
    plt.figure(figsize=(14.3, 7.7))
    plt.subplot(121)
    plot_matrix(MCAM, Y_LABEL, X_LABEL, ACC_TITLE)
    plt.subplot(122)
    plot_matrix(MCOAM, Y_LABEL, X_LABEL, ACC_ONE_LEVEL_TITLE)
    plt.suptitle(u"Estadísticas de Hombres", fontsize=18)
    plt.tight_layout()
    plt.subplots_adjust(top=0.86)
    if show:
        plt.show()
    else:
        plt.savefig("male.eps")

def plot_matrix(matrix, y_label, x_label, title):
    '''Plot rows of a given matrix'''
    rows, columns = matrix.shape
    x_vector = np.array([x+1 for x in range(columns)])
    for i in range(rows):
        plt.plot(x_vector, matrix[i, 0:columns])
    plt.title(title, fontsize=24)
    plt.ylabel(y_label, fontsize=24)
    plt.xlabel(x_label, fontsize=24)
    plt.tick_params(axis='both', which='major', labelsize=24)
    plt.tick_params(axis='both', which='minor', labelsize=24)
    plt.grid(True)

def append_matrices():
    '''Append female and male matrices to get total statistics'''
    global FULLCAM, FULLCOAM
    if len(MCAM) > 0 and len(FCAM) > 0:
        FULLCAM = np.vstack([FCAM, MCAM])
    if len(MCOAM) > 0 and len(FCOAM) > 0:
        FULLCOAM = np.vstack([FCOAM, MCOAM])

def append_matrices_gender(gender="NA", cumulative_acc=None, cumulative_one_level_acc=None, acc_by_percentage=None):
    '''Append the row vector data to the corresponding matrices'''
    global FCAM, MCAM, FCOAM, MCOAM, PACCM
    if gender.strip() == 'M':
        if len(FCAM) == 0:
            FCAM = cumulative_acc
        else:
            FCAM = np.vstack([FCAM, cumulative_acc])
        if len(FCOAM) == 0:
            FCOAM = cumulative_one_level_acc
        else:
            FCOAM = np.vstack([FCOAM, cumulative_one_level_acc])
    elif gender.strip() == 'H':
        if len(MCAM) == 0:
            MCAM = cumulative_acc
        else:
            MCAM = np.vstack([MCAM, cumulative_acc])
        if len(MCOAM) == 0:
            MCOAM = cumulative_one_level_acc
        else:
            MCOAM = np.vstack([MCOAM, cumulative_one_level_acc])
    if len(PACCM) == 0:
        PACCM = acc_by_percentage
    else:
        PACCM = np.vstack([PACCM, acc_by_percentage])

def get_gender_age(data=None):
    '''Get the gender and age information'''
    if data is not None:
        info = data[0].split(',')
        age = info[1]
        gender = info[2]
        return gender, age
    else:
        return 'NA', 'NA'

def process_file(data=None):
    '''Process the data file'''
    global PERCENTAGES
    percentages_correct = np.array([0.0, 0.0, 0.0, 0.0])
    percentage_count = np.array([0, 0, 0, 0])
    cumulative_acc = np.array([])
    cumulative_one_level_acc = np.array([])
    clean_data = data[2:len(data)-1]
    correct = 0.0
    correct_by_one_level = 0.0
    for i, entry in enumerate(clean_data):
        values = entry.split(',')
        user_answer = float(values[1])
        correct_answer = float(values[2])
        percentage_index_r, = np.where(PERCENTAGES == correct_answer)
        percentage_index = percentage_index_r[0]
        percentage_count[percentage_index] += 1
        if correct_answer == user_answer:
            correct += 1
            percentages_correct[percentage_index] += 1
        if abs(user_answer - correct_answer) == 0.25:
            correct_by_one_level += 1
        cumulative_acc = np.append(cumulative_acc, correct/(i+1))
        cumulative_one_level_acc = np.append(cumulative_one_level_acc, correct_by_one_level/(i+1))
    acc_by_percentage = np.divide(percentages_correct, percentage_count)
    return cumulative_acc*100, cumulative_one_level_acc*100, acc_by_percentage*100


if __name__ == "__main__":
    main()
