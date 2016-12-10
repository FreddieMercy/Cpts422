# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0019_remove_api_api_test'),
    ]

    operations = [
        migrations.AlterField(
            model_name='api',
            name='api_json',
            field=models.CharField(max_length=100),
            preserve_default=True,
        ),
    ]
