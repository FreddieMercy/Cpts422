# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations
import datetime
from django.utils.timezone import utc


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0002_auto_20150429_0938'),
    ]

    operations = [
        migrations.AddField(
            model_name='api',
            name='api_playlist',
            field=models.CharField(default=datetime.datetime(2015, 4, 29, 16, 39, 48, 368007, tzinfo=utc), max_length=100),
            preserve_default=False,
        ),
        migrations.AddField(
            model_name='api',
            name='api_usr',
            field=models.CharField(default='null', max_length=100),
            preserve_default=False,
        ),
    ]
