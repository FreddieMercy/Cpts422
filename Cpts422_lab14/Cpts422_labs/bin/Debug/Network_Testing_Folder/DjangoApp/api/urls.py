from django.conf.urls import url

from . import views

urlpatterns = [

    #url(r'^$', views.index)#, name='api_index'),
    url(r'^(?P<_id>[1-3])/$', views.index, name='api_index'),

]
