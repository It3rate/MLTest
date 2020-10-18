# mlp for multi-output regression
from numpy import mean
from numpy import std
import tensorflow as tf
#import tensorflow.compat.v1 as tf
from sklearn.datasets import make_regression
from sklearn.model_selection import RepeatedKFold
import pandas as pd
import numpy as np
import csv

train = False#True#

# get the dataset
def get_dataset():
	X = pd.read_csv("boxData/bx3_input.txt", header = 0).to_numpy()
	y = pd.read_csv("boxData/bx3_target.txt", header = 0).to_numpy()

	np.savetxt('boxData/testInputs.txt', X[:50], delimiter=',', fmt='%1.4f')
	np.savetxt('boxData/testTargets.txt', y[:50], delimiter=',', fmt='%1.4f')

	return X, y

# get the model
def get_model(n_inputs, n_outputs):
	model = tf.keras.Sequential([
	tf.keras.layers.Dense(20, input_dim=n_inputs, kernel_initializer='he_uniform', activation='relu'),
	tf.keras.layers.Dense(20, kernel_initializer='he_uniform', activation='relu'),
	tf.keras.layers.Dense(20, kernel_initializer='he_uniform', activation='relu'),
	tf.keras.layers.Dense(n_outputs)])

	model.compile(loss='mae', optimizer='adam')
	return model

# evaluate a model using repeated k-fold cross-validation
#def evaluate_model(X, y):
#	results = list()
#	n_inputs, n_outputs = X.shape[1], y.shape[1]
#	# define evaluation procedure
#	cv = RepeatedKFold(n_splits=3, n_repeats=3, random_state=1)
#	# enumerate folds
#	for train_ix, test_ix in cv.split(X):
#		# prepare data
#		X_train, X_test = X[train_ix], X[test_ix]
#		y_train, y_test = y[train_ix], y[test_ix]
#		# define model
#		model = get_model(n_inputs, n_outputs)
#		# fit model
#		model.fit(X_train, y_train, verbose=0, epochs=2)
#		# evaluate model on test set
#		mae = model.evaluate(X_test, y_test, verbose=0)
#		# store result
#		print('>%.3f' % mae)
#		results.append(mae)
#	return results

## load dataset
#X, y = get_dataset()
## evaluate model
#results = evaluate_model(X, y)
## summarize performance
#print('MAE: %.3f (%.3f)' % (mean(results), std(results)))

X, y = get_dataset()

if train:
	n_inputs, n_outputs = X.shape[1], y.shape[1]
	# get model
	model = get_model(n_inputs, n_outputs)
	# fit the model on all data
	model.fit(X, y, verbose=0, epochs=400)
	# make a prediction for new data
	model.save('boxData/boxModel')

if not train:	
	model = tf.keras.models.load_model('boxData/boxModel')

XTest = pd.read_csv("boxData/testInputs.txt", header=None).to_numpy()
yhat = model.predict(XTest)
print('Predicted: %s' % yhat)

np.savetxt('boxData/testPredictions.txt', yhat, delimiter=',', fmt='%1.4f')
