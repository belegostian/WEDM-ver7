# -*- coding: utf-8 -*-

import datetime
# import pickle as pk
import os
import json
import numpy as np
import pandas as pd
import tensorflow as tf
import matplotlib.pyplot as plt
# from pickle import load
from tensorflow import keras
from tensorflow.keras import layers
from keras.models import model_from_json

class LoanModel:

    #load model itself and additional tools
    def __init__(self):
        self.__model = None
        with open('C:/Users/user/source/repos/WEDM-ver6/WEDM-ver6/temp/model.json', 'r') as json_file:
            loaded_model_json = json_file.read()
            self.__model = model_from_json(loaded_model_json)
            self.__model.load_weights("C:/Users/user/source/repos/WEDM-ver6/WEDM-ver6/temp/Best-model.h5")
            self.__model.compile(optimizer=tf.keras.optimizers.Adam(), loss=keras.losses.MeanSquaredError(), metrics=[keras.metrics.MeanAbsoluteError()])

    def predict_this(self, json_arguments):
        #get JSON-packed parameters from the calls aggregator
        de_serialized_args = json.loads(json_arguments)
        pd_input = self.get_input_params( de_serialized_args["model_input"] )
        pd_input = np.array(pd_input)
        prediction = self.__model.predict(pd_input)
        return_obj = {
            "prediction" : str(prediction) ,
            "timestamp" : str(datetime.datetime.now())
        }

        return json.dumps(return_obj)
    
    
    def get_input_params(self, input_obj):
        tmp_list = list(input_obj.values())
        pd_input = np.array([tmp_list])
        
        return pd_input
