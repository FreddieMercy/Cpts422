#!/usr/bin/python3
from django.db import models
import datetime
from django.utils import timezone

class API(models.Model):
	api_text=models.CharField(max_length=200)
    	api_json = models.CharField(max_length=10000)
    	pub_date = models.DateTimeField('date published')
	def __unicode__(self):
		return self.api_text
        def was_published_recently(self):
                return self.pub_date >= timezone.now() - datetime.timedelta(days=1)
        
        was_published_recently.admin_order_field = 'pub_date'
    	was_published_recently.boolean = True
	was_published_recently.short_description = 'Published recently?'

#class Choice(models.Model):
#    	question = models.ForeignKey(Question)
#    	choice_text = models.CharField(max_length=200)
#    	votes = models.IntegerField(default=0)

#	def __unicode__(self):#
#		return self.choice_text
