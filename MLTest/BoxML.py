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
from tensorflow.python.framework.convert_to_constants import convert_variables_to_constants_v2

def get_dataset():
	X = pd.read_csv("boxModel/bx3_input.txt", header = 0).to_numpy()
	y = pd.read_csv("boxModel/bx3_target.txt", header = 0).to_numpy()

	np.savetxt('boxModel/testInputs.txt', X[:50], delimiter=',', fmt='%1.4f')
	np.savetxt('boxModel/testTargets.txt', y[:50], delimiter=',', fmt='%1.4f')

	return X, y

def get_model(n_inputs, n_outputs):
	model = tf.keras.Sequential([
	tf.keras.layers.Dense(20, input_dim=n_inputs, kernel_initializer='he_uniform', activation='relu', name='input'),
	tf.keras.layers.Dense(20, kernel_initializer='he_uniform', activation='relu'),
	tf.keras.layers.Dense(20, kernel_initializer='he_uniform', activation='relu'),
	tf.keras.layers.Dense(n_outputs, name='output')])

	model.compile(loss='mae', optimizer='adam')
	print(model.summary())
	return model

def freeze_and_save(v2Model):
	full_model = tf.function(lambda x: v2Model(x))
	full_model = full_model.get_concrete_function(
		x=tf.TensorSpec(v2Model.inputs[0].shape, 
				  v2Model.inputs[0].dtype, 
				  name="layoutInput"))
	frozen_func = convert_variables_to_constants_v2(full_model)
	frozen_func.graph.as_graph_def()
	layers = [op.name for op in frozen_func.graph.get_operations()]
	print(layers)
	tf.io.write_graph(graph_or_graph_def=frozen_func.graph, logdir="./boxModel", name="frozenBoxModel.pb", as_text=False)

def wrap_frozen_graph(graph_def, inputs, outputs, print_graph=False):
    def _imports_graph_def():
        tf.compat.v1.import_graph_def(graph_def, name="")

    wrapped_import = tf.compat.v1.wrap_function(_imports_graph_def, [])
    import_graph = wrapped_import.graph

    print("-" * 50)
    print("Frozen model layers: ")
    layers = [op.name for op in import_graph.get_operations()]
    if print_graph == True:
        for layer in layers:
            print(layer)
    print("-" * 50)

    return wrapped_import.prune(
        tf.nest.map_structure(import_graph.as_graph_element, inputs),
        tf.nest.map_structure(import_graph.as_graph_element, outputs))

def load_frozen_model():
	with tf.io.gfile.GFile("./boxModel/frozenBoxModel.pb", "rb") as f:
		graph_def = tf.compat.v1.GraphDef()
		loaded = graph_def.ParseFromString(f.read())
	return wrap_frozen_graph(graph_def=graph_def,
                                inputs=["layoutInput:0"],
                                outputs=["Identity:0"],
                                print_graph=True)

train = True #False #
useMLNetVersion = False #True #

if train:
	X, y = get_dataset()
	n_inputs, n_outputs = X.shape[1], y.shape[1]
	# get model
	model = get_model(n_inputs, n_outputs)
	# fit the model on all data
	model.fit(X, y, verbose=0, epochs=400)
	# make a prediction for new data
	model.save('boxModel/boxModel.h5')
	freeze_and_save(model) # For ML.net


if not train:	
	XTest = pd.read_csv("boxModel/testInputs.txt", header=None).to_numpy(dtype=np.float32)
	if useMLNetVersion:
		fr_modelfn = load_frozen_model();
		pred = fr_modelfn(x=tf.constant(XTest))
		print(pred);
		np.savetxt('boxModel/testPredictions.txt', pred[0], delimiter=',', fmt='%1.4f')
	else:
		model = tf.keras.models.load_model('boxModel/boxModel.h5')
		freeze_and_save(model) # For ML.net
		yhat = model(XTest)
		print('Predicted: %s' % yhat)
		np.savetxt('boxModel/testPredictions.txt', yhat, delimiter=',', fmt='%1.4f')

print(tf.__version__)
