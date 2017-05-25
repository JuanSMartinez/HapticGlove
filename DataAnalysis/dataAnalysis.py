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
    plot_m_accuracies()

def plot_full_accuracies_one_level():
    '''Plot all the cumulative accuracies by one level'''
    rows, columns = FULLCOAM.shape
    x_vector = np.array([x+1 for x in range(columns)])
    plt.figure()
    for i in range(rows):
        plt.plot(x_vector, FULLCOAM[i, 0:columns])
    plt.title("Porcentaje Acumulado de Aciertos Dentro de un Nivel de Diferencia")
    plt.ylabel("Porcentaje de Aciertos (%)")
    plt.xlabel("Intento")
    plt.show()

def plot_full_accuracies():
    '''Plot all the cumulative accuracies'''
    rows, columns = FULLCAM.shape
    x_vector = np.array([x+1 for x in range(columns)])
    plt.figure()
    for i in range(rows):
        plt.plot(x_vector, FULLCAM[i, 0:columns])
    plt.title("Porcentaje Acumulado de Aciertos")
    plt.ylabel("Porcentaje de Aciertos (%)")
    plt.xlabel("Intento")
    plt.show()

def plot_m_accuracies_one_level():
    '''Plot all the cumulative male accuracies by one level'''
    rows, columns = MCOAM.shape
    x_vector = np.array([x+1 for x in range(columns)])
    plt.figure()
    for i in range(rows):
        plt.plot(x_vector, MCOAM[i, 0:columns])
    plt.title("Porcentaje Acumulado de Aciertos en Hombres Dentro de un Nivel de Diferencia")
    plt.ylabel("Porcentaje de Aciertos (%)")
    plt.xlabel("Intento")
    plt.show()

def plot_f_accuracies_one_level():
    '''Plot all the cumulative female accuracies by one level'''
    rows, columns = FCOAM.shape
    x_vector = np.array([x+1 for x in range(columns)])
    plt.figure()
    for i in range(rows):
        plt.plot(x_vector, FCOAM[i, 0:columns])
    plt.title("Porcentaje Acumulado de Aciertos en Mujeres Dentro de un Nivel de Diferencia")
    plt.ylabel("Porcentaje de Aciertos (%)")
    plt.xlabel("Intento")
    plt.show()

def plot_m_accuracies():
    '''Plot all the cumulative male accuracies'''
    rows, columns = MCAM.shape
    x_vector = np.array([x+1 for x in range(columns)])
    plt.figure()
    for i in range(rows):
        plt.plot(x_vector, MCAM[i, 0:columns])
    plt.title("Porcentaje Acumulado de Aciertos en Hombres")
    plt.ylabel("Porcentaje de Aciertos (%)")
    plt.xlabel("Intento")
    plt.show()

def plot_f_accuracies():
    '''Plot all the cumulative female accuracies'''
    rows, columns = FCAM.shape
    x_vector = np.array([x+1 for x in range(columns)])
    plt.figure()
    for i in range(rows):
        plt.plot(x_vector, FCAM[i, 0:columns])
    plt.title("Porcentaje Acumulado de Aciertos en Mujeres")
    plt.ylabel("Porcentaje de Aciertos (%)")
    plt.xlabel("Intento")
    plt.show()

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
        if abs(user_answer - correct_answer) <= 0.25:
            correct_by_one_level += 1
        cumulative_acc = np.append(cumulative_acc, correct/(i+1))
        cumulative_one_level_acc = np.append(cumulative_one_level_acc, correct_by_one_level/(i+1))
    acc_by_percentage = np.divide(percentages_correct, percentage_count)
    return cumulative_acc*100, cumulative_one_level_acc*100, acc_by_percentage*100


if __name__ == "__main__":
    main()
