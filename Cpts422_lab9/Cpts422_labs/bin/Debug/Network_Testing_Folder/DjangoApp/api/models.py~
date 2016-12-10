from django.utils import timezone
from django.db import models
import datetime

class API(models.Model):
	api_text=models.CharField(max_length=200)
    	api_oauth=models.CharField(max_length=200)
	api_usr=models.CharField(max_length=100)
	api_playlist=models.CharField(max_length=100) 	
	edit_date = models.DateTimeField()
	api_json=models.TextField()
	api_keys=models.TextField()

	def __unicode__(self):
		return self.api_text
		
        def was_edit_recently(self):
                return self.edit_date >= timezone.now() - datetime.timedelta(days=1)
   	def save(self):
            	self.edit_date = datetime.datetime.today()
        	self.updated = datetime.datetime.today()
        	super(API, self).save()

        was_edit_recently.admin_order_field = 'edit_date'
    	was_edit_recently.boolean = True
	was_edit_recently.short_description = 'edit recently?'

