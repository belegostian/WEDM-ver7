
# -*- coding: utf-8 -*-

import os
import json
import numpy as np
import pandas as pd
import tensorflow as tf
import matplotlib.pyplot as plt
from tensorflow import keras
from tensorflow.keras import layers

data = pd.read_csv('C:/Users/user/source/repos/WEDM-ver7/WEDM-ver7/FeatureData.csv')
data.shape
data.dtypes


# 分為Train、validation和test，比例6:2:2
data_num = data.shape[0]
indexz = np.random.permutation(data_num)
train_indexz = indexz[:int(data_num *0.8)]
val_indexz = indexz[int(data_num *0.8):int(data_num *0.9)]
test_indexz = indexz[int(data_num *0.9):]

# 取出相對應索引data
train_data = data.iloc[train_indexz]
val_data = data.iloc[val_indexz]
test_data = data.iloc[test_indexz]

# 正規化

train_validation_data = pd.concat([train_data, val_data])
mean = train_validation_data.mean()
std = train_validation_data.std()
train_data = (train_data - mean) / std
val_data = (val_data - mean) / std

x_train = np.array(train_data.drop('drum_error',axis='columns'))
y_train = np.array(train_data['drum_error'])
x_val = np.array(val_data.drop('drum_error' ,axis='columns'))
y_val = np.array(val_data['drum_error'])

# model
model = keras.Sequential()
model.add(layers.Dense(32,activation='relu',input_shape=(23,)))
model.add(layers.Dense(64,activation='relu'))
model.add(layers.Dense(128,activation='relu'))
model.add(layers.Dense(1,activation='sigmoid'))
model.summary()

model.compile(optimizer=tf.keras.optimizers.Adam(), loss=keras.losses.MeanSquaredError(), metrics=[keras.metrics.MeanAbsoluteError()])


model_dir = 'C:/Users/user/source/repos/WEDM-ver7/WEDM-ver7/temp/'
log_dir = os.path.join('C:/Users/user/source/repos/WEDM-ver7/WEDM-ver7/temp/', 'Sequential')
os.makedirs(model_dir)

model_cbk = keras.callbacks.TensorBoard(log_dir=log_dir)
model_mckp = keras.callbacks.ModelCheckpoint(model_dir + '/Best-model.h5',monitor='val_mean_absolute_error',save_best_only=True,mode='min')


# 訓練模型
history = model.fit(x_train, y_train, batch_size=60, epochs=4000, validation_data=(x_val, y_val), callbacks=[model_cbk, model_mckp]) 

# 存檔
model_json = model.to_json()
with open("C:/Users/user/source/repos/WEDM-ver7/WEDM-ver7/temp/model.json", "w") as json_file:
    json_file.write(model_json)



# 驗證
history.history.keys()
plt.plot(history.history['loss'], label='train')
plt.plot(history.history['val_loss'], label='validation')
plt.title('Mean square error')
plt.ylabel('loss')
plt.xlabel('epochs')
plt.legend(loc='upper right')
plt.plot(history.history['mean_absolute_error'], label='train')
plt.plot(history.history['val_mean_absolute_error'], label='validation')
plt.title('Mean absolute error')
plt.ylabel('metrics')
plt.xlabel('epochs')
plt.legend(loc='upper right')


model = keras.models.load_model('C:/Users/user/source/repos/WEDM-ver7/WEDM-ver7/temp/Best-model.h5')
y_test = np.array(test_data['drum_error'])
test_data = (test_data - mean) / std
x_test = np.array(test_data.drop('drum_error', axis='columns'))
y_pred = model.predict(x_test)
y_pred = np.reshape(y_pred * std['drum_error'] + mean['drum_error'], y_test.shape)
percentage_error = np.mean(np.abs(y_test - y_pred)) / np.mean(y_test) * 100
print("Model_1 Percentage Error: {:.2f}%".format(percentage_error))
plt.plot(y_test,label='y_test')
plt.plot(y_pred,label='y_pred')
plt.legend()
plt.show()



# 全驗證
all_y_test = np.array(data['drum_error'])
all_test_mean = data.mean()
all_test_std = data.std()
all_test_data = (data-all_test_mean)/all_test_std
all_x_test = np.array(all_test_data.drop('drum_error',axis='columns'))
all_y_pred = model.predict(all_x_test)
all_y_pred = np.reshape(all_y_pred * all_test_std['drum_error'] + all_test_mean['drum_error'],all_y_test.shape)
percentage_error = []
for i in range(27):
    plt.plot(all_y_pred[6*i:6*(i+1)])
    plt.plot(all_y_test[6*i:6*(i+1)])
    percentage_error.append(np.mean(np.abs(all_y_test[6*i:6*(i+1)] - all_y_pred[6*i:6*(i+1)])) / np.mean(all_y_test[6*i:6*(i+1)]) * 100)
    plt.show()
percentage_error_all = np.mean(percentage_error)
print(percentage_error_all)
