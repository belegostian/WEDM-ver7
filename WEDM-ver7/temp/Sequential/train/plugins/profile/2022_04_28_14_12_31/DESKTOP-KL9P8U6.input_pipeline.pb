	?W[??????W[?????!?W[?????	Hs?9Ҽ7@Hs?9Ҽ7@!Hs?9Ҽ7@"e
=type.googleapis.com/tensorflow.profiler.PerGenericStepDetails$?W[??????Y??ڊ??A?`TR'???Y>?٬?\??*??????h@)       =2F
Iterator::Model=?U????!a?S?sU@)?/?'??1x?{???T@:Preprocessing2l
5Iterator::Model::ParallelMapV2::Zip[1]::ForeverRepeat?~j?t???!H?=T@)a2U0*???1?6ϗvv@:Preprocessing2v
?Iterator::Model::ParallelMapV2::Zip[0]::FlatMap[4]::Concatenate{?G?z??!??"?F@)???_vO~?1?
*L@:Preprocessing2Z
#Iterator::Model::ParallelMapV2::Zipy?&1???!??c?b,@)??ZӼ?t?1
*Lޭ@:Preprocessing2U
Iterator::Model::ParallelMapV2n??t?!???C?@)n??t?1???C?@:Preprocessing2?
OIterator::Model::ParallelMapV2::Zip[0]::FlatMap[4]::Concatenate[0]::TensorSlice??_?Le?!??u$???)??_?Le?1??u$???:Preprocessing2x
AIterator::Model::ParallelMapV2::Zip[1]::ForeverRepeat::FromTensora2U0*?c?!?6ϗvv??)a2U0*?c?1?6ϗvv??:Preprocessing2f
/Iterator::Model::ParallelMapV2::Zip[0]::FlatMap46<?R??!??]?+@)??H?}M?1?Ѷ??1??:Preprocessing:?
]Enqueuing data: you may want to combine small input data chunks into fewer but larger chunks.
?Data preprocessing: you may increase num_parallel_calls in <a href="https://www.tensorflow.org/api_docs/python/tf/data/Dataset#map" target="_blank">Dataset map()</a> or preprocess the data OFFLINE.
?Reading data from files in advance: you may tune parameters in the following tf.data API (<a href="https://www.tensorflow.org/api_docs/python/tf/data/Dataset#prefetch" target="_blank">prefetch size</a>, <a href="https://www.tensorflow.org/api_docs/python/tf/data/Dataset#interleave" target="_blank">interleave cycle_length</a>, <a href="https://www.tensorflow.org/api_docs/python/tf/data/TFRecordDataset#class_tfrecorddataset" target="_blank">reader buffer_size</a>)
?Reading data from files on demand: you should read data IN ADVANCE using the following tf.data API (<a href="https://www.tensorflow.org/api_docs/python/tf/data/Dataset#prefetch" target="_blank">prefetch</a>, <a href="https://www.tensorflow.org/api_docs/python/tf/data/Dataset#interleave" target="_blank">interleave</a>, <a href="https://www.tensorflow.org/api_docs/python/tf/data/TFRecordDataset#class_tfrecorddataset" target="_blank">reader buffer</a>)
?Other data reading or processing: you may consider using the <a href="https://www.tensorflow.org/programmers_guide/datasets" target="_blank">tf.data API</a> (if you are not using it now)?
:type.googleapis.com/tensorflow.profiler.BottleneckAnalysis?
host?Your program is HIGHLY input-bound because 23.7% of the total step time sampled is waiting for input. Therefore, you should first focus on reducing the input time.no*high2t28.4 % of the total step time sampled is spent on 'All Others' time. This could be due to Python execution overhead.9Hs?9Ҽ7@>Look at Section 3 for the breakdown of input time on the host.B?
@type.googleapis.com/tensorflow.profiler.GenericStepTimeBreakdown?
	?Y??ڊ???Y??ڊ??!?Y??ڊ??      ??!       "      ??!       *      ??!       2	?`TR'????`TR'???!?`TR'???:      ??!       B      ??!       J	>?٬?\??>?٬?\??!>?٬?\??R      ??!       Z	>?٬?\??>?٬?\??!>?٬?\??JCPU_ONLYYHs?9Ҽ7@b 